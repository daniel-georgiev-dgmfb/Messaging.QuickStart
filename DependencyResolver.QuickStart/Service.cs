using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Providers.Logging.Microsoft;

namespace DependencyResolver.QuickStart
{
    internal class Service : IService
    {
        private readonly IGWLogger<Service> _logger;
        private readonly IEnumerable<IRule> _rules;
        public Service(IEnumerable<IRule> rules, IGWLogger<Service> logger)
        {
            this._rules = rules;
            this._logger = logger;
            this._logger.LogInformation("Service instantiated. Dependencies: {0}, {1}", rules, logger);
        }
        public Task DoWork(object input)
        {
            this._logger.LogInformation("DoWork invoke on Service.");
            return Task.CompletedTask;
        }
    }
}