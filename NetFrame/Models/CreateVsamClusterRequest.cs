namespace NetFrame.Models
{
    public class CreateVsamClusterRequest
    {
        public string Dsorg { get; set; } = "KSDS";

        public int Primary { get; set; } = 10;

        public int Secondary { get; set; } = 5;

        public string Alcunit { get; set; } = "TRK";

        public string Recfm { get; set; } = "VB";

        public int? Keylen { get; set; }

        public int? Keyoff { get; set; }

        public int? Lrecl { get; set; }

        public string? Volser { get; set; }
    }
}
