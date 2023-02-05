using System;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Providers.Logging.Microsoft;

namespace Logging.QuickStart
{
    internal class ServiceNonGenericLogging : IOtherService
    {
        private readonly IGWLogger _logger;

        public ServiceNonGenericLogging(IGWLogger logger)
        {
            this._logger = logger;
        }
        public Task DoWork(object input)
        {
            this._logger.LogTrace("Trace message with parameters. Parameter: '{0}'", input);
            this._logger.LogDebug("Debug message with parameters. Parameter: '{0}'", input);
            this._logger.LogWarning("Warning message with parameters. Parameter: '{0}'", input);
            this._logger.LogInformation("Information message with parameters. Parameter: '{0}'", input);
            this._logger.LogInformation(123, "Information message with event id and parameters. Parameter: '{0}'", input);
            this._logger.LogInformation(1234, new NullReferenceException(), "Information message with event id, exception and parameters. Parameter: '{0}'", input);
            this._logger.LogError("Error message with parameters.", input);
            this._logger.LogError(new NullReferenceException(), "Error message with exception and parameters. Parameter: '{0}'", input);
            this._logger.LogCritical("Critical message with parameters. Parameter: '{0}'", input);
            return Task.CompletedTask;
        }
    }
}