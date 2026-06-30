using Microsoft.Extensions.Options;
using NetFrame.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ZosmfAuthHandler : DelegatingHandler
    {
        private readonly ZosmfConfig _config;

        public ZosmfAuthHandler(IOptions<ZosmfConfig> config)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null && !string.IsNullOrWhiteSpace(_config.Username))
            {
                var authInfo = $"{_config.Username}:{_config.Password}";
                var byteArray = Encoding.ASCII.GetBytes(authInfo);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
