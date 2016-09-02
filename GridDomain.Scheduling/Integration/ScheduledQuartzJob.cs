using System;
using System.IO;
using Akka.Actor;
using Akka.DI.Core;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Scheduling.Quartz.Logging;
using Quartz;
using Wire;

namespace GridDomain.Scheduling.Integration
{
    public class ScheduledCommandJobData
    {
        public ScheduleKey Key { get; }
        public CommandPlan Plan { get; }
        public DateTime RunAt { get; }

        public ScheduledCommandJobData(ScheduleKey key, CommandPlan plan, DateTime runAt)
        {
            Key = key;
            Plan = plan;
            RunAt = runAt;
        }
    }

    public class ScheduledQuartzJob : IJob
    {
        public const string ScheduledCommandDataKey = nameof(ScheduledCommandDataKey);
        private readonly IQuartzLogger _quartzLogger;
        private readonly IPublisher _publisher;
        private readonly ICommandExecutor _executor;


        public ScheduledQuartzJob(IQuartzLogger quartzLogger,
                                  ICommandExecutor executor,
                                  IPublisher publisher)
        {
            Condition.NotNull(() => quartzLogger);
            Condition.NotNull(() => executor);
            Condition.NotNull(() => publisher);

            _publisher = publisher;
            _executor = executor;
            _quartzLogger = quartzLogger;
        }

        public void Execute(IJobExecutionContext context)
        {
            var isFirstTimeFiring = true;
            try
            {
                isFirstTimeFiring = context.RefireCount == 0;
                var jobDataMap = context.JobDetail.JobDataMap;

                var prms = Deserialize<ScheduledCommandJobData>(jobDataMap[ScheduledCommandDataKey] as byte[]);

                try
                {
                    _publisher.Publish(new QuartzJobStarted(prms.Key, prms.Plan.Command));

                    var result = _executor.Execute<object>(prms.Plan).Result;

                    _publisher.Publish(new QuartzJobCompleted(prms.Key, result));
                }
                catch (Exception e)
                {
                    _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                    _publisher.Publish(new QuartzJobFault(prms.Key,e));
                    _publisher.Publish(CommandFault.NewTyped(prms.Plan.Command,e));
                    e.RethrowWithStackTrace();
                }
            }
            catch (Exception ex)
            {
                var jobExecutionException = new JobExecutionException(ex)
                {
                    RefireImmediately = isFirstTimeFiring,
                    UnscheduleAllTriggers = false,
                    UnscheduleFiringTrigger = false
                };
                throw jobExecutionException;
            }
        }

        public static IJobDetail Create(ScheduledCommandJobData data)
        {
            var jobDataMap = new JobDataMap{{ ScheduledCommandDataKey, Serialize(data) }};
            return CreateJob(data.Key, jobDataMap);
        }

        private static byte[] Serialize(object source)
        {
            using (var stream = new MemoryStream())
            {
                new Serializer().Serialize(source, stream);
                return stream.ToArray();
            }
        }

        private static T Deserialize<T>(byte[] source)
        {
            using (var stream = new MemoryStream(source))
            {
                return new Serializer().Deserialize<T>(stream);
            }
        }

        private static IJobDetail CreateJob(ScheduleKey key, JobDataMap jobDataMap)
        {
            var jobKey = new JobKey(key.Name, key.Group);
            return JobBuilder
                       .Create<ScheduledQuartzJob>()
                       .WithIdentity(jobKey)
                       .WithDescription(key.Description)
                       .UsingJobData(jobDataMap)
                       .RequestRecovery(true)
                       .Build();
        }
    }
}