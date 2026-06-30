namespace NetFrame.Models
{
    public class RetrieveContentOptions
    {
        public string? Search { get; set; }
        public string? Research { get; set; }
        public bool? Insensitive { get; set; }
        public int? MaxReturnSize { get; set; }
        public string? IfNoneMatch { get; set; }
        public ZosmfDataType DataType { get; set; } = ZosmfDataType.Text;
        public bool? ReturnEtag { get; set; }
        public MigratedRecallMode MigratedRecall { get; set; } = MigratedRecallMode.Wait;
        public string? RecordRange { get; set; }
        public EnqueueLock ObtainEnq { get; set; } = EnqueueLock.None;
        public string? SessionRef { get; set; }
        public bool? ReleaseEnq { get; set; }
        public string? TargetSystemUser { get; set; }
        public string? TargetSystemPassword { get; set; }
        public string? DsnameEncoding { get; set; }
    }
}