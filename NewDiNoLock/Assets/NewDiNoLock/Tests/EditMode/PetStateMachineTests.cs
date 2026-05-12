using System;
using NewDiNoLock.Core;
using NewDiNoLock.Infrastructure;
using NUnit.Framework;

namespace NewDiNoLock.Tests.EditMode
{
    public sealed class PetStateMachineTests
    {
        [Test]
        public void Create_DefaultsToIdleState()
        {
            var stateMachine = CreateStateMachine();

            Assert.AreEqual(PetState.Idle, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.Idle, stateMachine.CurrentPriority);
        }

        [Test]
        public void TryRequestState_WhenPriorityIsHigher_ChangesStateAndPublishesEvent()
        {
            var eventBus = new EventBus();
            var stateMachine = CreateStateMachine(eventBus);
            PetStateChangedEvent receivedEvent = default;
            var eventReceived = false;
            eventBus.Subscribe<PetStateChangedEvent>(eventData =>
            {
                receivedEvent = eventData;
                eventReceived = true;
            });

            var changed = stateMachine.TryRequestState(PetState.Walk, "auto walk");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Walk, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.Walk, stateMachine.CurrentPriority);
            Assert.IsTrue(eventReceived);
            Assert.AreEqual(PetState.Idle, receivedEvent.PreviousState);
            Assert.AreEqual(PetState.Walk, receivedEvent.CurrentState);
            Assert.AreEqual(PetActionPriority.Walk, receivedEvent.Priority);
            Assert.AreEqual("auto walk", receivedEvent.Reason);
        }

        [Test]
        public void TryRequestState_WhenCurrentStateIsDragged_RejectsWalkAndInteract()
        {
            var stateMachine = CreateStateMachine();
            stateMachine.TryRequestState(PetState.Dragged, "drag start");

            var walkChanged = stateMachine.TryRequestState(PetState.Walk, "auto walk");
            var interactChanged = stateMachine.TryRequestState(PetState.Interact, "click");

            Assert.IsFalse(walkChanged);
            Assert.IsFalse(interactChanged);
            Assert.AreEqual(PetState.Dragged, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.Dragged, stateMachine.CurrentPriority);
        }

        [Test]
        public void TryRequestNotify_WhenCurrentStateIsHidden_NormalPriorityIsRejected()
        {
            var stateMachine = CreateStateMachine();
            stateMachine.TryRequestState(PetState.Hidden, "user hide");

            var changed = stateMachine.TryRequestNotify(false, "normal reminder");

            Assert.IsFalse(changed);
            Assert.AreEqual(PetState.Hidden, stateMachine.CurrentState);
        }

        [Test]
        public void TryRequestState_WhenCurrentStateIsInteract_AllowsInteractRestart()
        {
            var eventBus = new EventBus();
            var stateMachine = CreateStateMachine(eventBus);
            var eventCount = 0;
            eventBus.Subscribe<PetStateChangedEvent>(_ => eventCount++);
            stateMachine.TryRequestState(PetState.Interact, "first click");

            var changed = stateMachine.TryRequestState(PetState.Interact, "second click");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Interact, stateMachine.CurrentState);
            Assert.AreEqual(2, eventCount);
        }

        [Test]
        public void TryRequestNotify_WhenHighPriority_InterruptsInteractButNotDragged()
        {
            var stateMachine = CreateStateMachine();
            stateMachine.TryRequestState(PetState.Interact, "click");

            var changedFromInteract = stateMachine.TryRequestNotify(true, "urgent reminder");

            Assert.IsTrue(changedFromInteract);
            Assert.AreEqual(PetState.Notify, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.HighPriorityNotify, stateMachine.CurrentPriority);

            stateMachine.TryRequestState(PetState.Dragged, "drag start");
            var changedFromDragged = stateMachine.TryRequestNotify(true, "urgent reminder");

            Assert.IsFalse(changedFromDragged);
            Assert.AreEqual(PetState.Dragged, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.Dragged, stateMachine.CurrentPriority);
        }

        [Test]
        public void TryExitState_WhenCurrentStateMatches_TransitionsToFallbackState()
        {
            var stateMachine = CreateStateMachine();
            stateMachine.TryRequestState(PetState.Dragged, "drag start");

            var changed = stateMachine.TryExitState(PetState.Dragged, PetState.Idle, false, "drag end");

            Assert.IsTrue(changed);
            Assert.AreEqual(PetState.Idle, stateMachine.CurrentState);
            Assert.AreEqual(PetActionPriority.Idle, stateMachine.CurrentPriority);
        }

        [Test]
        public void TryExitState_WhenExpectedStateDoesNotMatch_DoesNothing()
        {
            var eventBus = new EventBus();
            var stateMachine = CreateStateMachine(eventBus);
            var eventCount = 0;
            eventBus.Subscribe<PetStateChangedEvent>(_ => eventCount++);
            stateMachine.TryRequestState(PetState.Hidden, "user hide");
            eventCount = 0;

            var changed = stateMachine.TryExitState(PetState.Dragged, PetState.Idle, false, "wrong exit");

            Assert.IsFalse(changed);
            Assert.AreEqual(PetState.Hidden, stateMachine.CurrentState);
            Assert.AreEqual(0, eventCount);
        }

        private static PetStateMachine CreateStateMachine(IEventBus eventBus = null)
        {
            return new PetStateMachine(eventBus ?? new EventBus(), new TestLogger());
        }

        private sealed class TestLogger : ILogger
        {
            public void Debug(string message)
            {
            }

            public void Info(string message)
            {
            }

            public void Warning(string message)
            {
            }

            public void Error(string message)
            {
            }

            public void Error(Exception exception, string message = null)
            {
            }
        }
    }
}
