using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class USSService : IUSSSService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<USSService> _logger;

        public USSService(HttpClient httpClient, ILogger<USSService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<UssItem>> ListDirectoryAsync(string path, int? depth = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(path)}";
            if (depth.HasValue) url += $"&depth={depth.Value}";
            if (limit.HasValue) url += $"&limit={limit.Value}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                var dirRes = await response.Content.ReadFromJsonAsync<UssDirectoryResponse>(cancellationToken: cancellationToken);
                return dirRes?.Items ?? new List<UssItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing USS directory: {Path}", path);
                throw;
            }
        }

        public async Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(path)}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
                request.Headers.Add("X-IBM-Data-Type", "text");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync(cancellationToken) ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting USS file content: {Path}", path);
                throw;
            }
        }

        public async Task WriteFileContentAsync(string path, string content, bool isBinary = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(path)}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, url);
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
                request.Headers.Add("X-IBM-Data-Type", isBinary ? "binary" : "text");

                var encoding = new UTF8Encoding(false);
                request.Content = new StringContent(content ?? string.Empty, encoding, "text/plain");

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing USS file content: {Path}", path);
                throw;
            }
        }

        public async Task DeleteFileAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(path)}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
                if (recursive)
                {
                    request.Headers.Add("X-IBM-Option", "recursive");
                }

                var response = await _httpClient.SendAsync(request, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting USS file/directory: {Path}", path);
                throw;
            }
        }
    }
}
