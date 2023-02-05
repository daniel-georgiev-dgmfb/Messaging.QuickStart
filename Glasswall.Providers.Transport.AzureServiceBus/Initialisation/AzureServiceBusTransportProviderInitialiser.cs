using Glasswall.Common.Transport.Providers;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Resilience;
using Glasswall.Kernel.Transport;
using Glasswall.Providers.Resilience.Polly;
using Glasswall.Providers.Transport.AzureServiceBus.Providers;
using Glasswall.Providers.Transport.AzureServiceBus.Transport;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.Providers.Transport.AzureServiceBus.Initialisation
{
    [ExcludeFromCodeCoverage]
    public static class AzureServiceBusTransportProviderInitialiser
    {
        public static IDependencyResolver AddAzureServiceBusTransportProvider(this IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
                throw new ArgumentNullException(nameof(dependencyResolver));

            if(!dependencyResolver.Contains<IReadOnlyTransportProvider, AzureServiceBusReadOnlyProvider>())
                dependencyResolver.RegisterType<IReadOnlyTransportProvider, AzureServiceBusReadOnlyProvider>(Lifetime.Singleton);

            if (!dependencyResolver.Contains<IReadWriteTransportProvider, AzureServiceBusReadWriteProvider>())
                dependencyResolver.RegisterType<IReadWriteTransportProvider, AzureServiceBusReadWriteProvider>(Lifetime.Singleton);

            if (!dependencyResolver.Contains<IWriteOnlyTransportProvider, AzureServiceBusWriteOnlyProvider>())
                dependencyResolver.RegisterType<IWriteOnlyTransportProvider, AzureServiceBusWriteOnlyProvider>(Lifetime.Singleton);

            if (!dependencyResolver.Contains<IMessageListener, ServiceBusListener>())
                dependencyResolver.RegisterType<IMessageListener, ServiceBusListener>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITransportManager, ServiceBusTransportManager>())
                dependencyResolver.RegisterType<ITransportManager, ServiceBusTransportManager>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITransportDispatcher, ServiceBusTransportDispatcher>())
                dependencyResolver.RegisterType<ITransportDispatcher, ServiceBusTransportDispatcher>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ITransport, ServiceBusTransport>())
                dependencyResolver.RegisterType<ITransport, ServiceBusTransport>(Lifetime.Transient);

            if (!dependencyResolver.Contains<ICircuitBreaker, PollyCircuitBreaker>())
                dependencyResolver.RegisterType<ICircuitBreaker, PollyCircuitBreaker>(Lifetime.Transient);

            if (!dependencyResolver.Contains<PollyCircuitBreakerConfiguration, PollyCircuitBreakerConfiguration>())
                dependencyResolver.RegisterType<PollyCircuitBreakerConfiguration, PollyCircuitBreakerConfiguration>(Lifetime.Singleton);

            return dependencyResolver;
        }
    }
}