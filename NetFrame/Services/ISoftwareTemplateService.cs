using NetFrame.Models.Provisioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ISoftwareTemplateService
    {
        // --- Private Catalog (scc) Operations ---
        Task<SoftwareTemplateDetail> CreateTemplateAsync(SoftwareTemplateRequest request, CancellationToken cancellationToken = default);
        Task<SoftwareTemplateDetail> GetTemplateAsync(string objectId, CancellationToken cancellationToken = default);
        Task<TemplateListResponse> ListTemplatesAsync(CancellationToken cancellationToken = default);
        Task DeleteTemplateAsync(string objectId, CancellationToken cancellationToken = default);
        Task PublishTemplateAsync(string objectId, bool archiveExisting = false, bool ignoreTest = false, CancellationToken cancellationToken = default);
        Task<RunTemplateResponse> TestTemplateAsync(string objectId, RunTemplateRequest request, CancellationToken cancellationToken = default);

        // --- Published Catalog (psc) Operations ---
        Task<SoftwareTemplateDetail> GetPublishedTemplateAsync(string name, CancellationToken cancellationToken = default);
        Task<TemplateListResponse> ListPublishedTemplatesAsync(CancellationToken cancellationToken = default);
        Task<RunTemplateResponse> RunTemplateAsync(string name, RunTemplateRequest request, CancellationToken cancellationToken = default);
        
        // --- Utilities ---
        Task<List<PromptVariable>> GetPromptVariablesAsync(string objectId, bool isPublished = false, CancellationToken cancellationToken = default);
        Task<TemplateHistoryResponse> GetTemplateHistoryAsync(string objectId, bool isPublished = false, CancellationToken cancellationToken = default);
    }
}
