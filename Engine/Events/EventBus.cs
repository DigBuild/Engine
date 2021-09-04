using System;
using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Events
{
    /// <summary>
    /// An event bus.
    /// </summary>
    public sealed class EventBus
    {
        /// <summary>
        /// Registers a new subscriber for the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="handler">The event handler</param>
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Event += handler;
        }
        
        /// <summary>
        /// Unregisters a subscriber for the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="handler">The event handler</param>
        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Event -= handler;
        }

        /// <summary>
        /// Posts an event to all subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <param name="evt">The event</param>
        /// <returns>The event</returns>
        public TEvent Post<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Post(evt);
            return evt;
        }
        
        private sealed class SubscriptionSet<TEvent> where TEvent : IEvent
        {
            private static ConditionalWeakTable<EventBus, SubscriptionSet<TEvent>> Store { get; } = new();
            public static SubscriptionSet<TEvent> Get(EventBus bus) => Store.GetOrCreateValue(bus);

            public event Action<TEvent>? Event;

            public void Post(TEvent evt)
            {
                Event?.Invoke(evt);
            }
        }
    }
}