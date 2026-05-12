using System;
using NewDiNoLock.Infrastructure;

namespace NewDiNoLock.Core
{
    public sealed class PetStateMachine
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger _logger;

        public PetStateMachine(IEventBus eventBus, ILogger logger, PetState initialState = PetState.Idle)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CurrentState = initialState;
            CurrentPriority = ResolvePriority(initialState, false);
        }

        public PetState CurrentState { get; private set; }

        public PetActionPriority CurrentPriority { get; private set; }

        public bool TryRequestState(PetState requestedState, string reason = null)
        {
            return TryRequestStateInternal(requestedState, false, reason);
        }

        public bool TryRequestNotify(bool isHighPriority, string reason = null)
        {
            return TryRequestStateInternal(PetState.Notify, isHighPriority, reason);
        }

        public bool TryExitState(
            PetState expectedCurrentState,
            PetState nextState = PetState.Idle,
            bool nextStateIsHighPriorityNotify = false,
            string reason = null)
        {
            if (CurrentState != expectedCurrentState)
            {
                _logger.Debug($"Ignore exit request because current state is {CurrentState}, expected {expectedCurrentState}. Reason: {reason ?? "n/a"}");
                return false;
            }

            return TransitionTo(nextState, ResolvePriority(nextState, nextStateIsHighPriorityNotify), reason);
        }

        private bool TryRequestStateInternal(PetState requestedState, bool isHighPriorityNotify, string reason)
        {
            var requestedPriority = ResolvePriority(requestedState, isHighPriorityNotify);
            if (requestedState == CurrentState && requestedPriority == CurrentPriority)
            {
                if (!CanRestartCurrentState(requestedState))
                {
                    return false;
                }
            }

            if (requestedPriority < CurrentPriority)
            {
                _logger.Debug($"Reject state request {requestedState} because current state {CurrentState} has higher priority. Reason: {reason ?? "n/a"}");
                return false;
            }

            return TransitionTo(requestedState, requestedPriority, reason);
        }

        private bool TransitionTo(PetState nextState, PetActionPriority nextPriority, string reason)
        {
            var previousState = CurrentState;
            CurrentState = nextState;
            CurrentPriority = nextPriority;

            _logger.Debug($"Pet state changed: {previousState} -> {nextState}. Priority: {nextPriority}. Reason: {reason ?? "n/a"}");
            _eventBus.Publish(new PetStateChangedEvent(previousState, nextState, nextPriority, reason));
            return true;
        }

        private static bool CanRestartCurrentState(PetState state)
        {
            return state == PetState.Interact;
        }

        private static PetActionPriority ResolvePriority(PetState state, bool isHighPriorityNotify)
        {
            switch (state)
            {
                case PetState.Hidden:
                    return PetActionPriority.Hidden;
                case PetState.Dragged:
                    return PetActionPriority.Dragged;
                case PetState.Notify:
                    return isHighPriorityNotify ? PetActionPriority.HighPriorityNotify : PetActionPriority.Notify;
                case PetState.Interact:
                    return PetActionPriority.Interact;
                case PetState.Walk:
                    return PetActionPriority.Walk;
                case PetState.Idle:
                    return PetActionPriority.Idle;
                case PetState.Sleep:
                    return PetActionPriority.Sleep;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unsupported pet state.");
            }
        }
    }
}
