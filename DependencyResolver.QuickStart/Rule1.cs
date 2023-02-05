using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Providers.Logging.Microsoft;

namespace DependencyResolver.QuickStart
{
    internal class Rule1 : IRule
    {
        private readonly IGWLogger<Service> _logger;

        public Rule1(IGWLogger<Service> logger)
        {
            this._logger = logger;
            this._logger.LogInformation("Rule1 instantiated. Dependency: {0}", logger);
        }
        public Task ApplyRule()
        {
            this._logger.LogInformation("ApplyRule invoked on Rule1");
            return Task.CompletedTask;
        }
    }
}
