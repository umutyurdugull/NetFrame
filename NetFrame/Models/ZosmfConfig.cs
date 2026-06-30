namespace NetFrame.Models
{
    public class ZosmfConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool AllowInsecureConnections { get; set; } = false;
        public string? TrustedCertificateThumbprint { get; set; }
        public int TimeoutSeconds { get; set; } = 100;
        public int RetryCount { get; set; } = 3;
        public int PollingIntervalSeconds { get; set; } = 3;
        public int MaxPollingAttempts { get; set; } = 30;
        public double PollingBackoffFactor { get; set; } = 1.5;
    }
}
