using System.Diagnostics;

namespace NetFrame.Services
{
    public static class ZosmfTelemetry
    {
        public static readonly ActivitySource Source = new ActivitySource("NetFrame.Sdk", "1.0.4");
    }
}
