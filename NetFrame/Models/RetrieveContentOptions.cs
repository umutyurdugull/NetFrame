namespace NetFrame.Models
{
    public class RetrieveContentOptions
    {
        // search: veri seti icinde belirtilen metin
        public string? Search { get; set; }

        // research: veri seti icinde belirtilen regex 
        public string? Research { get; set; }


        //buyuk kucuk harfd dikkatine gore arama yapmak icin 
        public bool? Insensitive { get; set; }

        public int? MaxReturnSize { get; set; }

        public string? IfNoneMatch { get; set; }

        public string DataType { get; set; } = "text";

        public bool? ReturnEtag { get; set; }

        public string MigratedRecall { get; set; } = "wait";

        public string? RecordRange { get; set; }

        public string? ObtainEnq { get; set; }

        public string? SessionRef { get; set; }

        public bool? ReleaseEnq { get; set; }
        public string? TargetSystemUser { get; set; }

        public string? TargetSystemPassword { get; set; }
        public string? DsnameEncoding { get; set; }
    }
}