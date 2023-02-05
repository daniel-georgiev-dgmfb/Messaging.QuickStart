using System;
using Glasswall.Common.Transport.Context;

namespace Glasswall.Providers.Transport.AzureServiceBus.Context
{
    public class ServiceBusContext : TransportProviderContext, IEquatable<ServiceBusContext>
    {
        public string QueueName { get; }

        public ServiceBusContext(string queueName)
        {
            QueueName = queueName;
        }

        public bool Equals(ServiceBusContext other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(QueueName, other.QueueName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceBusContext) obj);
        }

        public override int GetHashCode()
        {
            return (QueueName != null ? QueueName.GetHashCode() : string.Empty.GetHashCode());
        }

        public static bool operator ==(ServiceBusContext context1, ServiceBusContext context2)
        {
            if (((object)context1) == null || ((object)context2) == null)
                return Equals(context1, context2);

            return context1.Equals(context2);
        }

        public static bool operator !=(ServiceBusContext context1, ServiceBusContext context2)
        {
            if (((object)context1) == null || ((object)context2) == null)
                return !Equals(context1, context2);

            return !(context1.Equals(context2));
        }
    }
}
