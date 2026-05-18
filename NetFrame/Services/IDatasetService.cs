using NetFrame.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IDatasetService
    {
        Task<List<string>> ListDatasetsAsync(string dsLevel, CancellationToken cancellationToken = default);

        Task<DatasetMemberResponse> ListDatasetMembersAsync(
            string datasetName,
            ListMembersOptions? options = null,
            CancellationToken cancellationToken = default);


        Task<string> RetrieveDatasetContentAsync(
            string datasetName,
            string memberName = null,
            RetrieveContentOptions options = null,
            CancellationToken cancellationToken = default);
    }
}
