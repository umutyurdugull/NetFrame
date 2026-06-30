namespace NetFrame.Models
{
    public class WriteContentOptions
    {
        public string? IfMatch { get; set; }
        public ZosmfDataType DataType { get; set; } = ZosmfDataType.Text;
        public string ContentType { get; set; } = "text/plain";
        public MigratedRecallMode MigratedRecall { get; set; } = MigratedRecallMode.Wait;
        public EnqueueLock ObtainEnq { get; set; } = EnqueueLock.None;
        public string? SessionRef { get; set; }
        public bool? ReleaseEnq { get; set; }
        public string? DsnameEncoding { get; set; }
        public string? TargetSystemUser { get; set; }
        public string? TargetSystemPassword { get; set; }
    }
}