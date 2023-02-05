using System;
using System.Collections.Generic;
using System.Text;
using Glasswall.Kernel.Data.Connection;
using Glasswall.Providers.Transport.AzureServiceBus.Context;

namespace ReceiverTest
{
    internal class ServiceBusConnectionStringProvider : IConnectionStringProvider<string>
    {
        public ServiceBusConnectionStringProvider()
        {
            
        }

        public string GetConnectionString()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionString<TContext>(TContext context)
        {
            var serviceBusContext = context as ServiceBusContext;
            if (serviceBusContext == null) throw new ArgumentNullException(nameof(serviceBusContext));
            return serviceBusContext.QueueName;
        }
    }
}
