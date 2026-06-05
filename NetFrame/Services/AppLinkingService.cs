using Microsoft.Extensions.Logging;
using NetFrame.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class AppLinkingService : IAppLinkingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AppLinkingService> _logger;

        public AppLinkingService(HttpClient httpClient, ILogger<AppLinkingService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AppLinkingResponse<object>> RegisterEventTypeAsync(RegisterEventTypeRequest requestBody, CancellationToken cancellationToken = default)
        {
            if (requestBody == null) throw new ArgumentNullException(nameof(requestBody));

            const string endpoint = "/zosmf/izual/rest/eventtype";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AppLinkingResponse<object>>(cancellationToken: cancellationToken) ?? new AppLinkingResponse<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering event type: {Id}", requestBody.Id);
                throw;
            }
        }

        public async Task<AppLinkingResponse<object>> RegisterEventHandlerAsync(string eventTypeId, RegisterEventHandlerRequest requestBody, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(eventTypeId)) throw new ArgumentException("Event type ID cannot be empty.", nameof(eventTypeId));
            if (requestBody == null) throw new ArgumentNullException(nameof(requestBody));

            var endpoint = $"/zosmf/izual/rest/handler?eventTypeId={Uri.EscapeDataString(eventTypeId)}";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(endpoint, requestBody, cancellationToken);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<AppLinkingResponse<object>>(cancellationToken: cancellationToken) ?? new AppLinkingResponse<object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering event handler: {Id} for event type: {EventTypeId}", requestBody.Id, eventTypeId);
                throw;
            }
        }

        public async Task<AppLinkingResponse<EligibleTasksResult>> GetEligibleTasksAsync(string eventTypeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(eventTypeId)) throw new ArgumentException("Event type ID cannot be empty.", nameof(eventTypeId));

            var endpoint = $"/zosmf/izual/rest/adm/getHandlerEligibleTasks?eventTypeId={Uri.EscapeDataString(eventTypeId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AppLinkingResponse<EligibleTasksResult>>(endpoint, cancellationToken);
                return response ?? throw new InvalidOperationException("Empty response received from get eligible tasks endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving eligible tasks for event type: {EventTypeId}", eventTypeId);
                throw;
            }
        }

        public async Task<AppLinkingResponse<List<HandlerDetail>>> ListHandlersAsync(string eventTypeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(eventTypeId)) throw new ArgumentException("Event type ID cannot be empty.", nameof(eventTypeId));

            var endpoint = $"/zosmf/izual/rest/handler?eventTypeId={Uri.EscapeDataString(eventTypeId)}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<AppLinkingResponse<List<HandlerDetail>>>(endpoint, cancellationToken);
                return response ?? throw new InvalidOperationException("Empty response received from list handlers endpoint.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing handlers for event type: {EventTypeId}", eventTypeId);
                throw;
            }
        }

        public async Task UnregisterEventHandlerAsync(string handlerId, string eventTypeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(handlerId)) throw new ArgumentException("Handler ID cannot be empty.", nameof(handlerId));
            if (string.IsNullOrWhiteSpace(eventTypeId)) throw new ArgumentException("Event type ID cannot be empty.", nameof(eventTypeId));

            var endpoint = $"/zosmf/izual/rest/handler/{Uri.EscapeDataString(handlerId)}?eventTypeId={Uri.EscapeDataString(eventTypeId)}";
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering event handler: {HandlerId} for event type: {EventTypeId}", handlerId, eventTypeId);
                throw;
            }
        }

        public async Task UnregisterEventTypeAsync(string eventTypeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(eventTypeId)) throw new ArgumentException("Event type ID cannot be empty.", nameof(eventTypeId));

            var endpoint = $"/zosmf/izual/rest/eventtype/{Uri.EscapeDataString(eventTypeId)}";
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering event type: {EventTypeId}", eventTypeId);
                throw;
            }
        }
    }
}
