using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetFrame.Models
{
    public class ZosJob
    {
        [JsonPropertyName("jobid")]
        public string? JobId { get; set; }

        [JsonPropertyName("jobname")]
        public string? JobName { get; set; }

        [JsonPropertyName("subsystem")]
        public string? Subsystem { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }

        [JsonPropertyName("retcode")]
        public string? RetCode { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("files-url")]
        public string? FilesUrl { get; set; }

        [JsonPropertyName("job-correlator")]
        public string? JobCorrelator { get; set; }

        [JsonPropertyName("phase")]
        public int? Phase { get; set; }

        [JsonPropertyName("phase-name")]
        public string? PhaseName { get; set; }

        [JsonPropertyName("exec-system")]
        public string? ExecSystem { get; set; }

        [JsonPropertyName("exec-member")]
        public string? ExecMember { get; set; }

        [JsonPropertyName("exec-submitted")]
        public string? ExecSubmitted { get; set; }

        [JsonPropertyName("exec-started")]
        public string? ExecStarted { get; set; }

        [JsonPropertyName("exec-ended")]
        public string? ExecEnded { get; set; }
    }

    public class ZosJobFile
    {
        [JsonPropertyName("jobid")]
        public string? JobId { get; set; }

        [JsonPropertyName("jobname")]
        public string? JobName { get; set; }

        [JsonPropertyName("subsystem")]
        public string? Subsystem { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("stepname")]
        public string? StepName { get; set; }

        [JsonPropertyName("procstep")]
        public string? ProcStep { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }

        [JsonPropertyName("ddname")]
        public string? DdName { get; set; }

        [JsonPropertyName("record-count")]
        public int? RecordCount { get; set; }

        [JsonPropertyName("byte-count")]
        public int? ByteCount { get; set; }

        [JsonPropertyName("records-url")]
        public string? RecordsUrl { get; set; }
    }

    public class JobFeedback
    {
        [JsonPropertyName("jobid")]
        public string? JobId { get; set; }

        [JsonPropertyName("jobname")]
        public string? JobName { get; set; }

        [JsonPropertyName("original-jobid")]
        public string? OriginalJobId { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("member")]
        public string? Member { get; set; }

        [JsonPropertyName("sysname")]
        public string? Sysname { get; set; }

        [JsonPropertyName("job-correlator")]
        public string? JobCorrelator { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("internal-code")]
        public string? InternalCode { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
