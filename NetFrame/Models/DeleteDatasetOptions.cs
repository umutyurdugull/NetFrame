namespace NetFrame.Models
{
    public class DeleteDatasetOptions
    {
        public bool? Purge { get; set; }

        public bool? Erase { get; set; }

        public string? Volser { get; set; }

        public EnqueueLock ObtainEnq { get; set; } = EnqueueLock.None;

        public string? TargetSystem { get; set; }

        public string? TargetSystemUser { get; set; }

        public string? TargetSystemPassword { get; set; }
    }
}
