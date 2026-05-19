namespace NetFrame.Models
{
    public class ListMembersOptions
    {
        // Query Parameters
        public string? Start { get; set; }
        public string? Pattern { get; set; }

        public int? MaxItems { get; set; } 

        public string Attributes { get; set; } = "member"; 

        public bool RequestTotalRows { get; set; } = false;

        public string MigratedRecall { get; set; } = "wait";
    }
}
