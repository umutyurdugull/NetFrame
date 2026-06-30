using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrame.Services
{
    public class ZosmfHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("X-CSRF-ZOSMF-HEADER"))
            {
                request.Headers.Add("X-CSRF-ZOSMF-HEADER", "zosmf");
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
