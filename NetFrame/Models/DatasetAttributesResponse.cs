namespace NetFrame.Models
{
    public class DatasetAttributesResponse
    {
        public string? DsName { get; set; }
        public string? Dsorg { get; set; }
        public string? Recfm { get; set; }
        public int? Lrecl { get; set; }
        public int? Blksize { get; set; }
        public string? Volser { get; set; }
        public long? SpaceAllocated { get; set; }
        public string? Devtype { get; set; }
    }
}
