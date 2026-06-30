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
    public class DatasetService : IDatasetService
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);
        private readonly HttpClient _httpClient;

        public DatasetService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<string>> ListDatasetsAsync(string dsLevel, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dsLevel))
            {
                throw new ArgumentException("Dataset level cannot be empty.", nameof(dsLevel));
            }

            var endpoint = $"/zosmf/restfiles/ds?dslevel={Uri.EscapeDataString(dsLevel)}";
            var response = await _httpClient.GetFromJsonAsync<DatasetListResponse>(endpoint, cancellationToken).ConfigureAwait(false);
            
            var datasetNames = new List<string>();
            if (response?.Items != null)
            {
                foreach (var item in response.Items)
                {
                    if (!string.IsNullOrEmpty(item.DsName))
                    {
                        datasetNames.Add(item.DsName);
                    }
                }
            }

            return datasetNames;
        }

        public async Task CreateDatasetAsync(
            string datasetName,
            CreateDatasetRequest requestBody,
            CreateDatasetOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty.", nameof(datasetName));
            }

            if (requestBody == null)
            {
                throw new ArgumentNullException(nameof(requestBody), "Request body cannot be null.");
            }

            options ??= new CreateDatasetOptions();
            var endpoint = $"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}";

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = JsonContent.Create(requestBody);

            if (!string.IsNullOrEmpty(options.TargetSystem))
            {
                request.Headers.Add("X-IBM-Target-System", options.TargetSystem);
            }

            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task<DatasetMemberResponse> ListDatasetMembersAsync(
            string datasetName,
            ListMembersOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty.", nameof(datasetName));
            }

            options ??= new ListMembersOptions();

            var queryParams = new List<string>(2);
            if (!string.IsNullOrEmpty(options.Start)) queryParams.Add($"start={Uri.EscapeDataString(options.Start)}");
            if (!string.IsNullOrEmpty(options.Pattern)) queryParams.Add($"pattern={Uri.EscapeDataString(options.Pattern)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var endpoint = $"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}/member{queryString}";

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            if (options.MaxItems.HasValue)
            {
                request.Headers.Add("X-IBM-Max-Items", options.MaxItems.Value.ToString());
            }

            var attributes = options.Attributes;
            if (options.RequestTotalRows)
            {
                attributes += ",total";
            }
            request.Headers.Add("X-IBM-Attributes", attributes);

            if (!string.IsNullOrEmpty(options.MigratedRecall))
            {
                request.Headers.Add("X-IBM-Migrated-Recall", options.MigratedRecall);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DatasetMemberResponse>(cancellationToken: cancellationToken).ConfigureAwait(false);
            return result ?? new DatasetMemberResponse();
        }

        public async Task<string?> RetrieveDatasetContentAsync(
            string datasetName,
            string? memberName = null,
            RetrieveContentOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty.", nameof(datasetName));
            }

            options ??= new RetrieveContentOptions();

            if (!string.IsNullOrEmpty(options.Search) && !string.IsNullOrEmpty(options.Research))
            {
                throw new ArgumentException("Search and research parameters cannot be specified together.");
            }

            var endpointBuilder = new StringBuilder($"/zosmf/restfiles/ds/{Uri.EscapeDataString(datasetName)}");
            if (!string.IsNullOrWhiteSpace(memberName))
            {
                endpointBuilder.Append($"({Uri.EscapeDataString(memberName)})");
            }

            var queryParams = new List<string>(4);
            if (!string.IsNullOrEmpty(options.Search))
                queryParams.Add($"search={Uri.EscapeDataString(options.Search)}");

            if (!string.IsNullOrEmpty(options.Research))
                queryParams.Add($"research={Uri.EscapeDataString(options.Research)}");

            if (options.Insensitive.HasValue)
                queryParams.Add($"insensitive={options.Insensitive.Value.ToString().ToLower()}");

            if (options.MaxReturnSize.HasValue)
                queryParams.Add($"maxreturnsize={options.MaxReturnSize.Value}");

            if (queryParams.Count > 0)
            {
                endpointBuilder.Append("?").Append(string.Join("&", queryParams));
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, endpointBuilder.ToString());
            if (!string.IsNullOrEmpty(options.IfNoneMatch) && string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("If-None-Match", options.IfNoneMatch);
            }

            request.Headers.Add("X-IBM-Data-Type", MapDataType(options.DataType));

            if (options.ReturnEtag.HasValue && options.ReturnEtag.Value && string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("X-IBM-Return-Etag", "true");
            }

            request.Headers.Add("X-IBM-Migrated-Recall", MapMigratedRecall(options.MigratedRecall));

            if (!string.IsNullOrEmpty(options.RecordRange))
            {
                request.Headers.Add("X-IBM-Record-Range", options.RecordRange);
            }

            var enqHeader = MapEnqueueLock(options.ObtainEnq);
            if (!string.IsNullOrEmpty(enqHeader))
            {
                request.Headers.Add("X-IBM-Obtain-ENQ", enqHeader);
            }

            if (!string.IsNullOrEmpty(options.SessionRef))
            {
                request.Headers.Add("X-IBM-Session-Ref", options.SessionRef);
            }

            if (options.ReleaseEnq.HasValue && options.ReleaseEnq.Value)
            {
                request.Headers.Add("X-IBM-Release-ENQ", "true");
            }

            if (!string.IsNullOrEmpty(options.DsnameEncoding))
            {
                request.Headers.Add("X-IBM-Dsname-Encoding", options.DsnameEncoding);
            }

            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.NotModified)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> WriteDatasetContentAsync(
            string datasetName,
            string? memberName = null,
            string content = "",
            string? volser = null,
            WriteContentOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(datasetName))
            {
                throw new ArgumentException("Dataset name cannot be empty.", nameof(datasetName));
            }

            options ??= new WriteContentOptions();
            var endpointBuilder = new StringBuilder("/zosmf/restfiles/ds/");
            if (!string.IsNullOrWhiteSpace(volser))
            {
                endpointBuilder.Append($"-({Uri.EscapeDataString(volser)})/");
            }

            endpointBuilder.Append(Uri.EscapeDataString(datasetName));

            if (!string.IsNullOrWhiteSpace(memberName))
            {
                endpointBuilder.Append($"({Uri.EscapeDataString(memberName)})");
            }

            using var request = new HttpRequestMessage(HttpMethod.Put, endpointBuilder.ToString());
            request.Content = new StringContent(content ?? string.Empty, Utf8NoBom, options.ContentType);

            if (!string.IsNullOrEmpty(options.IfMatch))
            {
                request.Headers.TryAddWithoutValidation("If-Match", options.IfMatch);
            }

            request.Headers.Add("X-IBM-Data-Type", MapDataType(options.DataType));
            request.Headers.Add("X-IBM-Migrated-Recall", MapMigratedRecall(options.MigratedRecall));

            var enqHeader = MapEnqueueLock(options.ObtainEnq);
            if (!string.IsNullOrEmpty(enqHeader))
            {
                request.Headers.Add("X-IBM-Obtain-ENQ", enqHeader);
            }

            if (!string.IsNullOrEmpty(options.SessionRef))
            {
                request.Headers.Add("X-IBM-Session-Ref", options.SessionRef);
            }

            if (options.ReleaseEnq.HasValue && options.ReleaseEnq.Value)
            {
                request.Headers.Add("X-IBM-Release-ENQ", "true");
            }

            if (!string.IsNullOrEmpty(options.DsnameEncoding))
            {
                request.Headers.Add("X-IBM-Dsname-Encoding", options.DsnameEncoding);
            }

            if (!string.IsNullOrEmpty(options.TargetSystemUser) && !string.IsNullOrEmpty(options.TargetSystemPassword))
            {
                request.Headers.Add("X-IBM-Target-System-User", options.TargetSystemUser);
                request.Headers.Add("X-IBM-Target-System-Password", options.TargetSystemPassword);
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        }

        private static string MapDataType(ZosmfDataType type) => type == ZosmfDataType.Binary ? "binary" : "text";

        private static string MapMigratedRecall(MigratedRecallMode mode) => mode switch
        {
            MigratedRecallMode.NoWait => "nowait",
            MigratedRecallMode.Error => "error",
            _ => "wait"
        };

        private static string? MapEnqueueLock(EnqueueLock lockMode) => lockMode switch
        {
            EnqueueLock.Shared => "shr",
            EnqueueLock.Exclusive => "excl",
            EnqueueLock.SharedUpdate => "shru",
            EnqueueLock.ExclusiveUpdate => "exclu",
            _ => null
        };
    }
}