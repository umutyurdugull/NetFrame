using System.Collections.Generic;

namespace NetFrame.Models
{
    public class DatasetCompareResult
    {
        public bool AreIdentical { get; set; }

        public List<string> SourceOnlyLines { get; set; } = new List<string>();

        public List<string> TargetOnlyLines { get; set; } = new List<string>();

        public List<string> IdenticalLines { get; set; } = new List<string>();

        public string DiffSummary { get; set; } = string.Empty;
    }
}
