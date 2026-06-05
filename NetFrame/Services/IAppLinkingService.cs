using NetFrame.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IAppLinkingService
    {
        Task<AppLinkingResponse<object>> RegisterEventTypeAsync(RegisterEventTypeRequest request, CancellationToken cancellationToken = default);
        
        Task<AppLinkingResponse<object>> RegisterEventHandlerAsync(string eventTypeId, RegisterEventHandlerRequest request, CancellationToken cancellationToken = default);

        Task<AppLinkingResponse<EligibleTasksResult>> GetEligibleTasksAsync(string eventTypeId, CancellationToken cancellationToken = default);

        Task<AppLinkingResponse<List<HandlerDetail>>> ListHandlersAsync(string eventTypeId, CancellationToken cancellationToken = default);

        Task UnregisterEventHandlerAsync(string handlerId, string eventTypeId, CancellationToken cancellationToken = default);

        Task UnregisterEventTypeAsync(string eventTypeId, CancellationToken cancellationToken = default);
    }
}
