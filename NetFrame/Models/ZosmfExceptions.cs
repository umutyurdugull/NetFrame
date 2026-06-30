using System;

namespace NetFrame.Models
{
    public class ZosmfException : Exception
    {
        public ZosmfException() : base() { }
        public ZosmfException(string message) : base(message) { }
        public ZosmfException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ZosmfNetworkException : ZosmfException
    {
        public ZosmfNetworkException() : base() { }
        public ZosmfNetworkException(string message) : base(message) { }
        public ZosmfNetworkException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ZosmfApiException : ZosmfException
    {
        public int StatusCode { get; }
        public string? MessageId { get; }
        public string? RawResponse { get; }

        public ZosmfApiException() : base("z/OSMF API Error occurred.") { }
        public ZosmfApiException(string message) : base(message) { }
        public ZosmfApiException(string message, Exception innerException) : base(message, innerException) { }

        public ZosmfApiException(int statusCode, string? messageId, string messageText, string? rawResponse = null)
            : base($"z/OSMF API Error: [{statusCode}] {messageId} - {messageText}")
        {
            StatusCode = statusCode;
            MessageId = messageId;
            RawResponse = rawResponse;
        }
    }
}
