using System;
using System.Runtime.CompilerServices;

namespace DigBuild.Engine.Events
{
    public sealed class EventBus
    {
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Event += handler;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Event -= handler;
        }

        public void Post<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var subscriptionSet = SubscriptionSet<TEvent>.Get(this);
            subscriptionSet.Post(evt);
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