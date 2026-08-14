using NetFrame.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IDb2RestService
    {
        Task<string?> AuthenticateAsync();
        Task<string> ExecuteSqlAsync(string sqlStatement);
        Task CreateServiceAsync(string serviceName, string sqlStatement, string collectionId = "default");
        Task<string> CallServiceAsync(string serviceName, Dictionary<string, object>? parameters, string collectionId = "default");
        Task<string> ListServicesAsync();
        Task DeleteServiceAsync(string serviceName, string collectionId = "default");

        Task<string> ExecuteSqlViaJclAsync(string sqlStatement, string? jobCard = null, CancellationToken cancellationToken = default);
        Task<List<Db2TableItem>> ListUserTablesAsync(string creator, string? jobCard = null, CancellationToken cancellationToken = default);
    }
}
