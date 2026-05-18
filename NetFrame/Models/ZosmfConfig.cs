namespace NetFrame.Models
{
    public class ZosmfConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool AllowInsecureConnections { get; set; } = false;
        public int PollingIntervalSeconds { get; set; } = 3;
        public int MaxPollingAttempts { get; set; } = 30;
    }
}
