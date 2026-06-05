using NetFrame.Models.CloudProvisioning;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface ICloudProvisioningService
    {
        Task<ObtainIpResponse> ObtainIpAddressAsync(ResourcePoolRequest<ObtainIpParams> request, CancellationToken cancellationToken = default);
        Task ReleaseIpAddressAsync(ResourcePoolRequest<ReleaseIpParams> request, CancellationToken cancellationToken = default);
        
        Task<ObtainPortResponse> ObtainPortAsync(ResourcePoolRequest<ObtainPortParams> request, CancellationToken cancellationToken = default);
        Task ReleasePortAsync(ResourcePoolRequest<ReleasePortParams> request, CancellationToken cancellationToken = default);

        Task<ObtainSnaResponse> ObtainSnaApplicationNameAsync(ResourcePoolRequest<ObtainSnaParams> request, CancellationToken cancellationToken = default);
        Task ReleaseSnaApplicationNameAsync(ResourcePoolRequest<ReleaseSnaParams> request, CancellationToken cancellationToken = default);

        Task<AddClassificationRuleResponse> AddWlmClassificationRuleAsync(WlmClassificationRequest request, CancellationToken cancellationToken = default);
        Task RemoveWlmClassificationRuleAsync(WlmClassificationRequest request, CancellationToken cancellationToken = default);

        Task<DatasetAttributesResponse> GetDatasetAttributesAsync(string tenantId, string templateName, string registryUuid, string? dsnType = null, string? size = null, CancellationToken cancellationToken = default);

        Task<CreateLparEntryResponse> CreateLparResourcePoolEntryAsync(string rdpId, LparEntryRequest request, CancellationToken cancellationToken = default);
        Task ModifyLparResourcePoolEntryAsync(string rdpId, string lparPoolId, LparEntryRequest request, CancellationToken cancellationToken = default);
        Task DeleteLparResourcePoolEntryAsync(string rdpId, string lparPoolId, CancellationToken cancellationToken = default);
        Task<List<LparEntryDetail>> ListLparResourcePoolEntriesAsync(string rdpId, CancellationToken cancellationToken = default);
        Task<LparEntryDetail> GetLparResourcePoolEntryAsync(string rdpId, string lparPoolId, CancellationToken cancellationToken = default);
        Task<LparEntryDetail> ObtainLparResourcePoolEntryAsync(ObtainLparRequest request, CancellationToken cancellationToken = default);
        Task ReleaseLparResourcePoolEntryAsync(ReleaseLparRequest request, CancellationToken cancellationToken = default);
    }
}
