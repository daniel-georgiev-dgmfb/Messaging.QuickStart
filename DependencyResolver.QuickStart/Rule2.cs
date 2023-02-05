using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Providers.Logging.Microsoft;

namespace DependencyResolver.QuickStart
{
    internal class Rule2 : IRule
    {
        private readonly IGWLogger<Service> _logger;

        public Rule2(IGWLogger<Service> logger)
        {
            this._logger = logger;
            this._logger.LogInformation("Rule2 instantiated.Dependency: {0}", logger);
        }
        public Task ApplyRule()
        {
            this._logger.LogInformation("ApplyRule invoked on Rule2.");
            return Task.CompletedTask;
        }
    }
}
