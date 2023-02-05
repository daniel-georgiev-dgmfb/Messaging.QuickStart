using System;
using System.Threading.Tasks;
using Glasswall.Common.Transport;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Message.Handling;
using Glasswall.Kernel.Resilience;
using Glasswall.Kernel.Serialisation;
using Glasswall.Kernel.Transport;
using Glasswall.Providers.Transport.AzureServiceBus.Context;
using Glasswall.Providers.Transport.AzureServiceBus.Transport;

namespace Glasswall.Providers.Transport.AzureServiceBus.Providers
{
    public abstract class AzureServiceBusTransportProvider
    {
        protected readonly ISerialiser Serialiser;
        protected readonly IHandlerResolver HandlerResolver;
        protected readonly IHandlerInvoker HandlerInvoker;

        private readonly IConnectionStringProvider<string> _connectionStringResolver;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IGWLogger<AzureServiceBusTransportProvider> _eventLogger;
        private readonly Func<ServiceBusContext, ITransportConfiguration> _configurationFactory;

        protected AzureServiceBusTransportProvider(IConnectionStringProvider<string> connectionStringResolver,
            ICircuitBreaker circuitBreaker,
            ISerialiser serialiser,
            IHandlerResolver handlerResolver, 
            IHandlerInvoker handlerInvoker,
            IGWLogger<AzureServiceBusTransportProvider> eventLogger,
            Func<ServiceBusContext, ITransportConfiguration> configurationFactory)
        {
            _connectionStringResolver = connectionStringResolver ?? throw new ArgumentNullException(nameof(connectionStringResolver));
            _circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
            Serialiser = serialiser ?? throw new ArgumentNullException(nameof(serialiser));
            _eventLogger = eventLogger ?? throw new ArgumentNullException(nameof(eventLogger));
            HandlerResolver = handlerResolver ?? throw new ArgumentNullException(nameof(handlerResolver));
            HandlerInvoker = handlerInvoker ?? throw new ArgumentNullException(nameof(handlerInvoker));
            this._configurationFactory = configurationFactory == null ? this.DefaultTansportConfiguration : configurationFactory;
        }

        protected abstract Task SetupInternal(ServiceBusContext context);

        protected abstract Task<ITransportManager> CreateManager(ServiceBusContext context);

        protected async Task<ITransportManager> CreateManager(ServiceBusContext context, Mode transportMode)
        {
            var transport = await CreateTransport(context, transportMode);
            return new ServiceBusTransportManager(transport, _circuitBreaker, _eventLogger);
        }

        private Task<ITransport> CreateTransport(ServiceBusContext context, Mode transportMode)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be Null or Empty", nameof(connectionString));

            var configuration = this._configurationFactory(context);
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            configuration.TransportMode = transportMode;
            var transport = new ServiceBusTransport(() => configuration, _eventLogger);

            return Task.FromResult<ITransport>(transport);
        }

        private  ITransportConfiguration DefaultTansportConfiguration(ServiceBusContext context)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Connection string cannot be Null or Empty", nameof(connectionString));
            return new TransportConfiguration
            {
                TransportConnection = new TransportConnection(connectionString, context.QueueName.ToLower()),
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
        }
    }
}