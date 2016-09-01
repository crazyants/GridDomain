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
    class CommandScheduleJobData
    {
        public ScheduleKey Key { get; set; }
        public CommandPlan Plan { get; set; }
        public DateTime RunAt { get;}

        public CommandScheduleJobData(ScheduleKey key, CommandPlan plan, DateTime runAt)
        {
            Key = key;
            Plan = plan;
            RunAt = runAt;
        }

        public void AddTo(JobDataMap map)
        {
            map.Add(nameof(Key), Serialize(Key));
            map.Add(nameof(Plan), Serialize(Plan));
            map.Add(nameof(RunAt), Serialize(RunAt));
        }

        public static CommandScheduleJobData Parse(JobDataMap map)
        {
            return new CommandScheduleJobData(
                         Deserialize<ScheduleKey>(map[nameof(Key)] as byte[]),
                         Deserialize<CommandPlan>(map[nameof(Plan)] as byte[]),
                         Deserialize<DateTime>(map[nameof(RunAt)] as byte[])
                );
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
    }

    public class ScheduledQuartzJob : IJob
    {
        private const string CommandKey = nameof(CommandKey);
        private const string MessageKey = nameof(MessageKey);
        private const string ScheduleKey = nameof(ScheduleKey);
        private const string ExecutionOptionsKey = nameof(ExecutionOptionsKey);

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
               
                var jobData = CommandScheduleJobData.Parse(jobDataMap);
                var key = jobData.Key;

                try
                {
                    _publisher.Publish(new QuartzJobStarted(key, jobData.Plan.Command));

                    var result = _executor.Execute<object>(jobData.Plan).Result;

                    _publisher.Publish(new QuartzJobCompleted(key, result));
                }
                catch (Exception e)
                {
                    _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                    _publisher.Publish(new QuartzJobFault(key,e));
                    _publisher.Publish(CommandFault.NewTyped(jobData.Plan.Command,e));
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

        public static IJobDetail Create(ScheduleKey key, Command command, CommandExecutionOptions executionOptions)
        {
            var serializedCommand = Serialize(command);
            var serializedKey = Serialize(key);
            var serializedOptions = Serialize(executionOptions);

            var jobDataMap = new JobDataMap
            {
                { CommandKey, serializedCommand },
                { ScheduleKey, serializedKey },
                { ExecutionOptionsKey, serializedOptions }
            };
            return CreateJob(key, jobDataMap);
        }

        public static IJobDetail Create(ScheduleKey key, object message)
        {
            return CreateJob(key, new JobDataMap
                                  {
                                      { MessageKey, Serialize(message) },
                                      { ScheduleKey, Serialize(key) }
                                  });
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

        private static Command GetCommand(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[CommandKey] as byte[];
            return Deserialize<Command>(bytes);
        }

        private static ScheduleKey GetScheduleKey(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ScheduleKey] as byte[];
            return Deserialize<ScheduleKey>(bytes);
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