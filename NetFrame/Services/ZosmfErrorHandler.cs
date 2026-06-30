using NetFrame.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ZosmfErrorHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new ZosmfNetworkException("Mainframe connection failed or timed out.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                using (response)
                {
                    string rawContent = string.Empty;
                    try
                    {
                        rawContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                        
                        // Attempt to parse standard IBM error schema
                        var node = JsonNode.Parse(rawContent);
                        if (node != null)
                        {
                            string? messageId = node["messageId"]?.ToString() ?? node["msgId"]?.ToString();
                            string? messageText = node["messageText"]?.ToString() ?? node["msgText"]?.ToString() ?? node["message"]?.ToString();

                            if (!string.IsNullOrEmpty(messageText))
                            {
                                throw new ZosmfApiException((int)response.StatusCode, messageId, messageText, rawContent);
                            }
                        }
                    }
                    catch (Exception ex) when (ex is ZosmfApiException or OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        // Fall back if parsing fails
                    }

                    string safeContent = rawContent;
                    if (!string.IsNullOrEmpty(safeContent) && safeContent.Contains("Basic ", StringComparison.OrdinalIgnoreCase))
                    {
                        safeContent = "[REDACTED - SENSITIVE HEADER VALUES PRESENT]";
                    }

                    // Generic fallback error
                    throw new ZosmfApiException(
                        (int)response.StatusCode,
                        response.StatusCode.ToString(),
                        $"HTTP error occurred: {response.ReasonPhrase}. Details: {safeContent}",
                        safeContent);
                }
            }

            return response;
        }
    }
}
