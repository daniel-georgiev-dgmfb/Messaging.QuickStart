using System;
using System.Collections.Generic;
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
    public class AzureServiceBusReadWriteProvider : AzureServiceBusTransportProvider, IReadWriteTransportProvider
    {
        private readonly Dictionary<string, ITransportDispatcher> _dispatchers;

        public AzureServiceBusReadWriteProvider(
            IConnectionStringProvider<string> connectionStringResolver,
            ICircuitBreaker circuitBreaker,
            ISerialiser serialiser,
            IHandlerResolver handlerResolver,
            IHandlerInvoker handlerInvoker,
            IGWLogger<AzureServiceBusReadWriteProvider> eventLogger,
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
            _dispatchers = new Dictionary<string, ITransportDispatcher>();
        }

        public async Task Setup<TContext>(TContext context)
            where TContext : TransportProviderContext
        {
            var serviceBusContext = context as ServiceBusContext;
            if (serviceBusContext == null) throw new InvalidCastException($"Unable to cast {typeof(TContext)} to {typeof(ServiceBusContext)}");
            await SetupInternal(serviceBusContext);
        }

        public async Task<ITransportDispatcher> GetDispatcher<TContext>(TContext context) 
            where TContext : TransportProviderContext
        {
            var serviceBusContext = context as ServiceBusContext;
            if (serviceBusContext == null) throw new InvalidCastException($"Unable to cast {typeof(TContext)} to {typeof(ServiceBusContext)}");
            return await GetDispatcher(serviceBusContext);
        }

        protected override async Task SetupInternal(ServiceBusContext context)
        {
            var manager = await CreateManager(context);
            var dispatcher = new ServiceBusTransportDispatcher(manager, Serialiser);
            _dispatchers.Add(context.QueueName, dispatcher);
            await manager.Start(CancellationToken.None);
        }

        private async Task<ITransportDispatcher> GetDispatcher(ServiceBusContext context)
        {
            if (!_dispatchers.ContainsKey(context.QueueName))
                await Setup(context);

            return _dispatchers[context.QueueName];
        }

        protected override async Task<ITransportManager> CreateManager(ServiceBusContext context)
        {
            var manager = await CreateManager(context, Mode.SendReceive);
            var listener = new ServiceBusListener(Serialiser, HandlerResolver, HandlerInvoker);

            await manager.RegisterListener(listener);
            return manager;
        }
    }
}