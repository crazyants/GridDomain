using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Aggregates.CommonDomain;

namespace GridDomain.Aggregates
{
    namespace CommonDomain
    {
        using System;

        public interface IRouteEvents
        {
            void Register<T>(Action<T> handler);
            void Register(IAggregate aggregate);

            void Dispatch(object eventMessage);
        }
    }

    public class ConventionEventRouter : IRouteEvents
    {
        readonly bool throwOnApplyNotFound;
        private readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        private IAggregate registered;

        public ConventionEventRouter() : this(true)
        {
        }

        public ConventionEventRouter(bool throwOnApplyNotFound)
        {
            this.throwOnApplyNotFound = throwOnApplyNotFound;
        }

        public ConventionEventRouter(bool throwOnApplyNotFound, IAggregate aggregate) : this(throwOnApplyNotFound)
        {
            Register(aggregate);
        }

        public virtual void Register<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            this.Register(typeof(T), @event => handler((T)@event));
        }

        public virtual void Register(IAggregate aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException("aggregate");

            this.registered = aggregate;

            // Get instance methods named Apply with one parameter returning void
            var applyMethods = aggregate.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == "Apply" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof(void))
                .Select(m => new
                {
                    Method = m,
                    MessageType = m.GetParameters().Single().ParameterType
                });

            foreach (var apply in applyMethods)
            {
                var applyMethod = apply.Method;
                this.handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] { m as object }));
            }
        }

        public virtual void Dispatch(object eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException("eventMessage");

            Action<object> handler;
            if (this.handlers.TryGetValue(eventMessage.GetType(), out handler))
                handler(eventMessage);
            else if (this.throwOnApplyNotFound)
                this.registered.ThrowHandlerNotFound(eventMessage);
        }

        private void Register(Type messageType, Action<object> handler)
        {
            this.handlers[messageType] = handler;
        }
    }


    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException()
        { }

        public HandlerForDomainEventNotFoundException(string message)
            : base(message)
        { }

        public HandlerForDomainEventNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public HandlerForDomainEventNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }

    internal static class ExtensionMethods
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format ?? string.Empty, args);
        }

        public static void ThrowHandlerNotFound(this IAggregate aggregate, object eventMessage)
        {
            string exceptionMessage =
                "Aggregate of type '{0}' raised an event of type '{1}' but not handler could be found to handle the message."
                    .FormatWith(aggregate.GetType().Name, eventMessage.GetType().Name);

            throw new HandlerForDomainEventNotFoundException(exceptionMessage);
        }
    }

    public interface IAggregate
        {
            Guid Id { get; }
            int Version { get; }

            void ApplyEvent(object @event);
            ICollection GetUncommittedEvents();
            void ClearUncommittedEvents();
        }

    public abstract class AggregateBase : IAggregate, IEquatable<IAggregate>
    {
        private readonly ICollection<object> uncommittedEvents = new LinkedList<object>();

        private IRouteEvents registeredRoutes;

        protected AggregateBase()
            : this(null)
        { }

        protected AggregateBase(IRouteEvents handler)
        {
            if (handler == null)
            {
                return;
            }

            this.RegisteredRoutes = handler;
            this.RegisteredRoutes.Register(this);
        }

        protected IRouteEvents RegisteredRoutes
        {
            get
            {
                return this.registeredRoutes ?? (this.registeredRoutes = new ConventionEventRouter(true, this));
            }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("AggregateBase must have an event router to function");
                }

                this.registeredRoutes = value;
            }
        }

        public Guid Id { get; protected set; }

        public int Version { get; protected set; }

        void IAggregate.ApplyEvent(object @event)
        {
            this.RegisteredRoutes.Dispatch(@event);
            this.Version++;
        }

        ICollection IAggregate.GetUncommittedEvents()
        {
            return (ICollection)this.uncommittedEvents;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            this.uncommittedEvents.Clear();
        }


        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == this.Id;
        }

        protected void Register<T>(Action<T> route)
        {
            this.RegisteredRoutes.Register(route);
        }

        protected void RaiseEvent(object @event)
        {
            ((IAggregate)this).ApplyEvent(@event);
            this.uncommittedEvents.Add(@event);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IAggregate);
        }
    }
}
