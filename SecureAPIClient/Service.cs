using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Web;
using Glasswall.Kernel.Web.Authorisation;
using Glasswall.Platform.Web.Api.Client;

namespace SecureAPIClient
{
    internal class Service : IService
    {

        private readonly IApiClient _httpClient;
        private readonly IGWLogger<Service> _logger;

        public Service(IApiClient httpClient, IGWLogger<Service> logger)
        {
            
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this._httpClient = httpClient;
            this._logger = logger;
        }
        public async Task<string> GetResource(RequestContext request, CancellationToken cancellationToken)
        {
            try
            {
                return await this._httpClient.GetAsync(request, cancellationToken);
               
            }
            catch (HttpRequestException e)
            {
                this._logger.Log(LogLevel.Error, 0, String.Empty, e, (s, ex) => ex.ToString());
                throw;
            }
        }
    }
}