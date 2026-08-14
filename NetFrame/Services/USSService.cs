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
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);
        private readonly HttpClient _httpClient;

        public USSService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var dirRes = await response.Content.ReadFromJsonAsync<UssDirectoryResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return dirRes?.Items ?? new List<UssItem>();
        }

        public async Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var cleanPath = path.StartsWith("/") ? path : "/" + path;
            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(cleanPath)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-IBM-Data-Type", "text");

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        }

        public async Task WriteFileContentAsync(string path, string content, bool isBinary = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var cleanPath = path.StartsWith("/") ? path : "/" + path;
            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(cleanPath)}";

            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("X-IBM-Data-Type", isBinary ? "binary" : "text");
            request.Content = new StringContent(content ?? string.Empty, Utf8NoBom, "text/plain");

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFileAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var cleanPath = path.StartsWith("/") ? path : "/" + path;
            var url = $"/zosmf/restfiles/fs{cleanPath}";

            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (recursive)
            {
                request.Headers.Add("X-IBM-Option", "recursive");
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                var queryUrl = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(cleanPath)}";
                using var fallbackReq = new HttpRequestMessage(HttpMethod.Delete, queryUrl);
                if (recursive)
                {
                    fallbackReq.Headers.Add("X-IBM-Option", "recursive");
                }
                using var fallbackRes = await _httpClient.SendAsync(fallbackReq, cancellationToken).ConfigureAwait(false);
                fallbackRes.EnsureSuccessStatusCode();
            }
        }

        public Task<List<UssItem>> ListUssDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            return ListDirectoryAsync(path, null, null, cancellationToken);
        }

        public Task DeleteUssFileAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
        {
            return DeleteFileAsync(path, recursive, cancellationToken);
        }

        public async Task CreateUssDirectoryAsync(string path, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            var cleanPath = path.StartsWith("/") ? path : "/" + path;
            var url = $"/zosmf/restfiles/fs{cleanPath}";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-IBM-Option", "directory");
            request.Content = JsonContent.Create(new { request = "directory" });

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task ChangeUssPermissionsAsync(string path, string mode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(mode))
            {
                throw new ArgumentException("Path and mode cannot be null or empty.");
            }

            var cleanPath = path.StartsWith("/") ? path : "/" + path;
            var formattedMode = mode.Length == 3 ? "0" + mode : mode;
            var url = $"/zosmf/restfiles/fs?path={Uri.EscapeDataString(cleanPath)}";

            using var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("X-IBM-Option", "chmod");
            request.Content = JsonContent.Create(new { request = "chmod", mode = formattedMode });

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }
    }
}
