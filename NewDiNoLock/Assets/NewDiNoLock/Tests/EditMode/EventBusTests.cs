using System;
using NewDiNoLock.Infrastructure;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class EventBusTests
    {
        [Test]
        public void Publish_NotifiesSubscribedHandler()
        {
            var eventBus = new EventBus();
            var receivedValue = 0;

            eventBus.Subscribe<TestEvent>(eventData => receivedValue = eventData.Value);
            eventBus.Publish(new TestEvent(42));

            Assert.AreEqual(42, receivedValue);
        }

        [Test]
        public void DisposeSubscription_StopsFutureNotifications()
        {
            var eventBus = new EventBus();
            var callCount = 0;

            var subscription = eventBus.Subscribe<TestEvent>(_ => callCount++);
            eventBus.Publish(new TestEvent(1));
            subscription.Dispose();
            eventBus.Publish(new TestEvent(2));

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void DisposeDuringPublish_DoesNotNotifyDisposedSubscriptionAgain()
        {
            var eventBus = new EventBus();
            var firstCallCount = 0;
            var secondCallCount = 0;
            IDisposable secondSubscription = null;

            eventBus.Subscribe<TestEvent>(_ =>
            {
                firstCallCount++;
                secondSubscription.Dispose();
            });
            secondSubscription = eventBus.Subscribe<TestEvent>(_ => secondCallCount++);

            eventBus.Publish(new TestEvent(1));
            eventBus.Publish(new TestEvent(2));

            Assert.AreEqual(2, firstCallCount);
            Assert.AreEqual(0, secondCallCount);
        }

        [Test]
        public void DifferentEventTypes_DoNotShareHandlers()
        {
            var eventBus = new EventBus();
            var testEventCallCount = 0;
            var otherEventCallCount = 0;

            eventBus.Subscribe<TestEvent>(_ => testEventCallCount++);
            eventBus.Subscribe<OtherEvent>(_ => otherEventCallCount++);

            eventBus.Publish(new TestEvent(1));

            Assert.AreEqual(1, testEventCallCount);
            Assert.AreEqual(0, otherEventCallCount);
        }

        private readonly struct TestEvent
        {
            public TestEvent(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        private readonly struct OtherEvent
        {
        }
    }
}
