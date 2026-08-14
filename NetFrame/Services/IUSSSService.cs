using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetFrame.Models;

namespace NetFrame.Services
{
    public interface IUSSSService
    {
        Task<List<UssItem>> ListDirectoryAsync(string path, int? depth = null, int? limit = null, CancellationToken cancellationToken = default);
        Task<string> GetFileContentAsync(string path, CancellationToken cancellationToken = default);
        Task WriteFileContentAsync(string path, string content, bool isBinary = false, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);

        Task<List<UssItem>> ListUssDirectoryAsync(string path, CancellationToken cancellationToken = default);
        Task DeleteUssFileAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);
        Task CreateUssDirectoryAsync(string path, CancellationToken cancellationToken = default);
        Task ChangeUssPermissionsAsync(string path, string mode, CancellationToken cancellationToken = default);
    }
}
