using System.Diagnostics;

namespace NetFrame.Services
{
    public static class ZosmfTelemetry
    {
        public static readonly ActivitySource Source = new ActivitySource("NetFrame.Sdk", "2.0.0");
    }
}
