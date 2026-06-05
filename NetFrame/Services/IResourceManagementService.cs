using NetFrame.Models.ResourceManagement;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IResourceManagementService
    {
        // Domain Operations
        Task<DomainDetail> GetDomainAsync(string objectId, CancellationToken cancellationToken = default);
        Task<DomainHistoryResponse> GetDomainHistoryAsync(string domainId, CancellationToken cancellationToken = default);
        Task<DomainListResponse> ListDomainsAsync(CancellationToken cancellationToken = default);

        // Tenant Operations
        Task<CreateTenantResponse> CreateTenantAsync(string domainId, CreateTenantRequest request, CancellationToken cancellationToken = default);
        Task<TenantDetail> GetTenantAsync(string objectId, CancellationToken cancellationToken = default);
        Task<DomainHistoryResponse> GetTenantHistoryAsync(string tenantId, CancellationToken cancellationToken = default);
        Task<TenantListResponse> ListTenantsAsync(CancellationToken cancellationToken = default);
        Task DeleteTenantAsync(string tenantId, CancellationToken cancellationToken = default);

        Task AssignCpuCappingPropertiesAsync(string tenantId, AssignCpuCappingRequest request, CancellationToken cancellationToken = default);
        Task AssignMemoryCappingPropertiesAsync(string tenantId, AssignMemoryCappingRequest request, CancellationToken cancellationToken = default);
        Task AssignSolutionIdAsync(string tenantId, AssignSolutionIdRequest request, CancellationToken cancellationToken = default);

        Task DisableCpuCappingAsync(string tenantId, CancellationToken cancellationToken = default);
        Task DisableMemoryCappingAsync(string tenantId, CancellationToken cancellationToken = default);
        Task DisableMeteringAsync(string tenantId, CancellationToken cancellationToken = default);

        Task EnableCpuCappingAsync(string tenantId, CancellationToken cancellationToken = default);
        Task EnableMemoryCappingAsync(string tenantId, CancellationToken cancellationToken = default);
        Task EnableMeteringAsync(string tenantId, CancellationToken cancellationToken = default);

        Task AddTenantConsumersAsync(string tenantId, TenantConsumerActionRequest request, CancellationToken cancellationToken = default);
        Task RemoveTenantConsumersAsync(string tenantId, TenantConsumerActionRequest request, CancellationToken cancellationToken = default);

        Task AddTenantDescriptionAsync(string tenantId, TenantDescriptionActionRequest request, CancellationToken cancellationToken = default);
        
        Task AddTenantGroupsAsync(string tenantId, TenantGroupActionRequest request, CancellationToken cancellationToken = default);
        Task RemoveTenantGroupsAsync(string tenantId, TenantGroupActionRequest request, CancellationToken cancellationToken = default);

        // Resource Pool Operations
        Task<ResourcePoolDetail> GetResourcePoolAsync(string tenantId, string rdpId, CancellationToken cancellationToken = default);
    }
}
