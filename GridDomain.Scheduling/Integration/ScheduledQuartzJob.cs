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
    public class ScheduledQuartzJob : IJob
    {
        private const string CommandKey = nameof(CommandKey);
        private const string EventKey = nameof(EventKey);
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
                var command = GetCommand(jobDataMap);
                var options = GetExecutionOptions(jobDataMap);
                var key = GetScheduleKey(jobDataMap);
                try
                {
                    _publisher.Publish(new QuartzJobStarted(key.Name, key.Group, command));

                    var expect = ExpectedMessage.Once(options.SuccessEventType, options.MessageIdFieldName,
                        options.SuccessMessageId);
                    var plan = new CommandPlan(command, options.Timeout, expect);
                    var result = _executor.Execute<object>(plan).Result;

                    _publisher.Publish(new QuartzJobCompleted(key.Name, key.Group, result));
                }
                catch (Exception e)
                {
                    _quartzLogger.LogFailure(context.JobDetail.Key.Name, e);
                    _publisher.Publish(new QuartzJobFault(key.Name,key.Group,e));
                    _publisher.Publish(CommandFault.NewTyped(command,e));
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

        public static IJobDetail Create(ScheduleKey key, Command command, ExtendedExecutionOptions executionOptions)
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

        public static IJobDetail Create(ScheduleKey key, DomainEvent eventToSchedule)
        {
            var serializedEvent = Serialize(eventToSchedule);
            var serializedKey = Serialize(key); var jobDataMap = new JobDataMap
            {
                { EventKey, serializedEvent },
                { ScheduleKey, serializedKey }
            };
            return CreateJob(key, jobDataMap);
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

        private static ExtendedExecutionOptions GetExecutionOptions(JobDataMap jobDatMap)
        {
            var bytes = jobDatMap[ExecutionOptionsKey] as byte[];
            return Deserialize<ExtendedExecutionOptions>(bytes);
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