namespace NewDiNoLock.Core
{
    public enum PetActionPriority
    {
        Sleep = 0,
        Idle = 10,
        Walk = 20,
        Notify = 25,
        Interact = 30,
        HighPriorityNotify = 40,
        Dragged = 50,
        Hidden = 60
    }
}
