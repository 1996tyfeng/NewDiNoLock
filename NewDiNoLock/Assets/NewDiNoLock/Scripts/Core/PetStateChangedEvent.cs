namespace NewDiNoLock.Core
{
    public readonly struct PetStateChangedEvent
    {
        public PetStateChangedEvent(PetState previousState, PetState currentState, PetActionPriority priority, string reason)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Priority = priority;
            Reason = reason;
        }

        public PetState PreviousState { get; }

        public PetState CurrentState { get; }

        public PetActionPriority Priority { get; }

        public string Reason { get; }
    }
}
