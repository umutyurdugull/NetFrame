namespace NetFrame.Models
{
    public enum ZosmfDataType
    {
        Text,
        Binary
    }

    public enum MigratedRecallMode
    {
        Wait,
        NoWait,
        Error
    }

    public enum EnqueueLock
    {
        None,
        Shared,
        Exclusive,
        SharedUpdate,
        ExclusiveUpdate
    }
}
