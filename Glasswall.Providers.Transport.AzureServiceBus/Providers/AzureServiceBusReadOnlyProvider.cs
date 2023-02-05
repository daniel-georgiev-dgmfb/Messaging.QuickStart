using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Common.Transport.Context;
using Glasswall.Common.Transport.Providers;
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
    public class AzureServiceBusReadOnlyProvider : AzureServiceBusTransportProvider, IReadOnlyTransportProvider
    {
        public AzureServiceBusReadOnlyProvider(
            IConnectionStringProvider<string> connectionStringResolver,
            ICircuitBreaker circuitBreaker,
            ISerialiser serialiser,
            IHandlerResolver handlerResolver,
            IHandlerInvoker handlerInvoker,
            IGWLogger<AzureServiceBusReadOnlyProvider> eventLogger,
            Func<ServiceBusContext, ITransportConfiguration> configurationFactory = null)
            : base(
                connectionStringResolver,
                circuitBreaker,
                serialiser,
                handlerResolver,
                handlerInvoker,
                eventLogger,
                configurationFactory)
        {
        }

        public async Task Setup<TContext>(TContext context) 
            where TContext : TransportProviderContext
        {
            var serviceBusContext = context as ServiceBusContext;
            if (serviceBusContext == null) throw new InvalidCastException($"Unable to cast {typeof(TContext)} to {typeof(ServiceBusContext)}");
            await SetupInternal(serviceBusContext);
        }

        protected override async Task SetupInternal(ServiceBusContext context)
        {
            var manager = await CreateManager(context);
            await manager.Start(CancellationToken.None);
        }

        protected override async Task<ITransportManager> CreateManager(ServiceBusContext context)
        {
            var manager = await CreateManager(context, Mode.ReceiveOnly);
            var listener = new ServiceBusListener(Serialiser, HandlerResolver, HandlerInvoker);

            await manager.RegisterListener(listener);
            return manager;
        }
    }
}