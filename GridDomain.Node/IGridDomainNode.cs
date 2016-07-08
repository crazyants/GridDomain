using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Node
{
    //TODO: add execution track status
    public interface IGridDomainNode
    {
        void Execute(params ICommand[] commands);
     
        Task<T> Execute<T>(ICommand command, params ExpectedMessage[] expect);
    }


    public static class GridNodeExtensions
    {
        public static Task<T> Execute<T>(this IGridDomainNode node, CommandAndConfirmation data)
        {
            return node.Execute<T>(data.Command, data.ExpectedMessages);
        }

        public static T Execute<T>(this IGridDomainNode node, CommandAndConfirmation command, TimeSpan timeout)
        {
            var commandExecutionTask = node.Execute<T>(command);
            if (!commandExecutionTask.Wait(timeout))
                throw new TimeoutException($"Command execution timed out");
            return commandExecutionTask.Result;
        }

        public static void Execute(this IGridDomainNode node, CommandAndConfirmation command, TimeSpan timeout)
        {
            var commandExecutionTask = node.Execute<object>(command);
            if (!commandExecutionTask.Wait(timeout))
                throw new TimeoutException($"Command execution timed out");
        }


        public static void Execute(this IGridDomainNode node, ICommand command, TimeSpan timeout, params ExpectedMessage[] expect)
        {
            Execute(node, new CommandAndConfirmation(command, expect), timeout);
        }
    }
}