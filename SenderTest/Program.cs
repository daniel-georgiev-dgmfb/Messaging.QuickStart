using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Glasswall.Common.MessageHandling.Extensions;
using Glasswall.Common.Serialisation.Binary.Extensions;
using Glasswall.Common.Transport.Providers;
using Glasswall.Kernel.Configuration;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Kernel.DependencyResolver;
using Glasswall.Providers.Logging.Console;
using Glasswall.Providers.Logging.Microsoft;
using Glasswall.Providers.Microsoft.DependencyInjection;
using Glasswall.Providers.Transport.AzureServiceBus.Context;
using Glasswall.Providers.Transport.AzureServiceBus.Initialisation;
using Messaging;

namespace SenderTest
{
    public static class ObjectExtensions
    {
        public static void M1(this string o)
        {
            if (o == null)
                throw new InvalidOperationException(nameof(o));
        }
    }
    class Program
    {
        private static Task Foo()
        {
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            try
            {
                var r = false;
                var t1 = await Program.Foo()
                    .ContinueWith(async t =>
                    {
                        if (t.IsFaulted)
                            throw t.Exception;
                        await Task.Delay(100);
                        r = true;
                    });
                await t1;
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
                await dependencyResolver.Initialise();
                var readOnlyTransportProvider = dependencyResolver.Resolve<IWriteOnlyTransportProvider>();
                var context = new ServiceBusContext("Endpoint=sb://adminconsolebus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=tWhwQISufrzuQgdmUH3xGAWS0/t6gy9vRNOAGJ0Q1lE=;EntityPath=commandqueue");
                var dispatcher = await readOnlyTransportProvider.GetDispatcher(context);
                var cancellationTokenSource = new CancellationTokenSource();
                var s = (string)null;
                s.M1();
                for (var i = 0; i < 1; i++)
                {
                    var message = new TestMessage(Guid.NewGuid()) { Name = $"Name:{i}" };
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required))
                    {
                        try
                        {
                            dispatcher.SendMessage(message, cancellationTokenSource.Token).GetAwaiter().GetResult();
                            transaction.Complete();
                        }
                        catch(Exception e)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

    }
}
