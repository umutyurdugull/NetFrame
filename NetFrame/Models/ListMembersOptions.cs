namespace NetFrame.Models
{
    public class ListMembersOptions
    {
        // Query Parameters
        public string? Start { get; set; }
        public string? Pattern { get; set; }

        // Custom Headers
        /// <summary>
        /// X-IBM-Max-Items: Maksimum dönecek öğe sayısı. 0 tüm öğeleri döndürür. Varsayılan 1000'dir.
        /// </summary>
        public int? MaxItems { get; set; } 

        /// <summary>
        /// X-IBM-Attributes: "member" (varsayılan) veya detaylı özellikler için "base"
        /// </summary>
        public string Attributes { get; set; } = "member"; 

        /// <summary>
        /// X-IBM-Attributes sonuna ",total" eklenmesini kontrol eder.
        /// </summary>
        public bool RequestTotalRows { get; set; } = false;

        /// <summary>
        /// X-IBM-Migrated-Recall: "wait" (varsayılan), "nowait" veya "error"
        /// </summary>
        public string MigratedRecall { get; set; } = "wait";
    }
}
