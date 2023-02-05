using System;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Providers.Logging.Microsoft;

namespace Logging.QuickStart
{
    internal class TransactionalService : ITransactionalService
    {
        private readonly IGWLogger _logger;

        public TransactionalService(IGWLogger logger)
        {
            this._logger = logger;
        }
        public Task DoWork(object input)
        {
            Console.WriteLine("\r\n\r\nDemo logging for filetrust microservices(with non generic loggin) .......");

            var ex = new NullReferenceException();
            var state = new TransactionalState(Guid.NewGuid(), "Message to log", null);
            var errorState = new TransactionalState(Guid.NewGuid(), "Error message to log", ex);
            this._logger.Log(LogLevel.Trace, 0, state, null, TransactionalState.MessageFormat);
            this._logger.Log(LogLevel.Debug, 1, state, null, TransactionalState.MessageFormat);
            this._logger.Log(LogLevel.Information, 2, state, null, TransactionalState.MessageFormat);
            this._logger.Log(LogLevel.Warning, 3, state, null, TransactionalState.MessageFormat);
            this._logger.Log(LogLevel.Error, 4, errorState, ex, TransactionalState.MessageFormat);
            this._logger.Log(LogLevel.Critical, 5, errorState, ex, TransactionalState.MessageFormat);
            return Task.CompletedTask;
        }
    }
}