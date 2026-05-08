using System;
using System.Collections.Generic;

namespace NewDiNoLock.Infrastructure
{
    public interface IEventBus
    {
        IDisposable Subscribe<TEvent>(Action<TEvent> handler);
        void Publish<TEvent>(TEvent eventData);
    }

    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, object> _subscriptions = new Dictionary<Type, object>();

        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var eventType = typeof(TEvent);
            if (!_subscriptions.TryGetValue(eventType, out var eventSubscriptionsObject))
            {
                var eventSubscriptions = new EventSubscriptionList<TEvent>();
                _subscriptions.Add(eventType, eventSubscriptions);
                return eventSubscriptions.Subscribe(handler);
            }

            return ((EventSubscriptionList<TEvent>)eventSubscriptionsObject).Subscribe(handler);
        }

        public void Publish<TEvent>(TEvent eventData)
        {
            if (_subscriptions.TryGetValue(typeof(TEvent), out var eventSubscriptionsObject))
            {
                ((EventSubscriptionList<TEvent>)eventSubscriptionsObject).Publish(eventData);
            }
        }

        private sealed class EventSubscriptionList<TEvent>
        {
            private readonly List<EventSubscription> _subscriptions = new List<EventSubscription>();
            private int _publishingDepth;
            private bool _needsPrune;

            public IDisposable Subscribe(Action<TEvent> handler)
            {
                var subscription = new EventSubscription(this, handler);
                _subscriptions.Add(subscription);
                return subscription;
            }

            public void Publish(TEvent eventData)
            {
                _publishingDepth++;
                var initialCount = _subscriptions.Count;
                try
                {
                    for (var index = 0; index < initialCount; index++)
                    {
                        var subscription = _subscriptions[index];
                        if (!subscription.IsDisposed)
                        {
                            subscription.Invoke(eventData);
                        }
                    }
                }
                finally
                {
                    _publishingDepth--;
                    if (_publishingDepth == 0 && _needsPrune)
                    {
                        PruneDisposed();
                    }
                }
            }

            private void Unsubscribe(EventSubscription subscription)
            {
                subscription.MarkDisposed();

                if (_publishingDepth > 0)
                {
                    _needsPrune = true;
                    return;
                }

                _subscriptions.Remove(subscription);
            }

            private void PruneDisposed()
            {
                _needsPrune = false;

                for (var index = _subscriptions.Count - 1; index >= 0; index--)
                {
                    if (_subscriptions[index].IsDisposed)
                    {
                        _subscriptions.RemoveAt(index);
                    }
                }
            }

            private sealed class EventSubscription : IDisposable
            {
                private readonly EventSubscriptionList<TEvent> _owner;
                private Action<TEvent> _handler;

                public EventSubscription(EventSubscriptionList<TEvent> owner, Action<TEvent> handler)
                {
                    _owner = owner;
                    _handler = handler;
                }

                public bool IsDisposed { get; private set; }

                public void Dispose()
                {
                    if (IsDisposed)
                    {
                        return;
                    }

                    _owner.Unsubscribe(this);
                }

                public void Invoke(TEvent eventData)
                {
                    _handler(eventData);
                }

                public void MarkDisposed()
                {
                    IsDisposed = true;
                    _handler = null;
                }
            }
        }
    }
}
