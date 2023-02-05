using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Common.MessageHandling.Extensions;
using Glasswall.Common.Serialisation.Binary.Extensions;
using Glasswall.Common.Transport;
using Glasswall.Common.Transport.Providers;
using Glasswall.Kernel.Configuration;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Kernel.Message.Handling;
using Glasswall.Kernel.Transport;
using Glasswall.Providers.Logging.Console;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Microsoft.DependencyInjection;
using Glasswall.Providers.Transport.AzureServiceBus.Context;
using Glasswall.Providers.Transport.AzureServiceBus.Initialisation;
using Messaging;

namespace ReceiverTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var dependencyResolver = new MicrosoftDependencyInjection();
                dependencyResolver
                   .AddLogging()
                   .AddConsole()
                    .AddAzureServiceBusTransportProvider()
                    .AddMessageHandling()
                    .AddBinarySerialisation();
                dependencyResolver.RegisterType<IConnectionStringProvider<string>, ServiceBusConnectionStringProvider>(Lifetime.Singleton);
                dependencyResolver.RegisterInstance<IDependencyResolver>(dependencyResolver, Lifetime.Singleton);
                dependencyResolver.RegisterInstance<IConfiguration>(new Configuration(), Lifetime.Singleton);
                dependencyResolver.RegisterType<IMessageHandler<TestMessage>, TestMessageHandler>(Lifetime.Transient);
                dependencyResolver.RegisterFactory<Func<ServiceBusContext, ITransportConfiguration>>(() =>
                {
                    return c =>
                    {
                        var connectionStringResolver = dependencyResolver.Resolve<IConnectionStringProvider<string>>();
                        var connectionString = connectionStringResolver.GetConnectionString(c);
                        if (string.IsNullOrEmpty(connectionString))
                            throw new ArgumentException("Connection string cannot be Null or Empty", nameof(connectionString));
                        return new TransportConfiguration
                        {
                            TransportConnection = new TransportConnection(connectionString, c.QueueName.ToLower()),
                            MaxDegreeOfParallelism = 20
                        };
                    };
                }, Lifetime.Singleton);
                await dependencyResolver.Initialise();
                var readOnlyTransportProvider = dependencyResolver.Resolve<IReadOnlyTransportProvider>();
                var context = new ServiceBusContext("Endpoint=sb://adminconsolebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=tWhwQISufrzuQgdmUH3xGAWS0/t6gy9vRNOAGJ0Q1lE=;EntityPath=commandqueue");
                await readOnlyTransportProvider.Setup(context);
                await Task.Delay(Timeout.Infinite);
            }
            catch(Exception e)
            {
                throw;
            }

        }
    }
}