using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public interface IDb2TokenStore : IDisposable
    {
        Task<string> GetOrAuthenticateAsync(Func<Task<string>> authenticator);
        void InvalidateToken();
    }

    public class Db2TokenStore : IDb2TokenStore
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private string? _token;
        private bool _disposed;

        public async Task<string> GetOrAuthenticateAsync(Func<Task<string>> authenticator)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!string.IsNullOrEmpty(_token))
            {
                return _token;
            }

            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                if (string.IsNullOrEmpty(_token))
                {
                    _token = await authenticator().ConfigureAwait(false);
                }
                return _token;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void InvalidateToken()
        {
            _token = null;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _semaphore.Dispose();
                _disposed = true;
            }
        }
    }
}
