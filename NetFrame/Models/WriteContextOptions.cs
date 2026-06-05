namespace NetFrame.Models
{
    public class WriteContentOptions
    {
        public string? IfMatch { get; set; }

        public string DataType { get; set; } = "text";

        public string ContentType { get; set; } = "text/plain";

        public string MigratedRecall { get; set; } = "wait";

        public string? ObtainEnq { get; set; }

        public string? SessionRef { get; set; }

        public bool? ReleaseEnq { get; set; }

        public string? DsnameEncoding { get; set; }

        public string? TargetSystemUser { get; set; }

        public string? TargetSystemPassword { get; set; }
    }
}