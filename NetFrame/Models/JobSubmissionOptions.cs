namespace NetFrame.Models
{
    public class JobSubmissionOptions
    {
        public string? DatasetPath { get; set; }
        public string? JclContent { get; set; }
        public string? LocalFilePath { get; set; }
        public string? DestinationDataset { get; set; }
        public string? DestinationMember { get; set; }
        public string? IntrdrMode { get; set; } = "TEXT";
    }
}
