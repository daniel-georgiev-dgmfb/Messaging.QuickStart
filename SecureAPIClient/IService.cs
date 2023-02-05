using System.Threading;
using System.Threading.Tasks;
using Glasswall.Platform.Web.Api.Client;

namespace SecureAPIClient
{
    internal interface IService
    {
        Task<string> GetResource(RequestContext request, CancellationToken cancellationToken);
    }
}