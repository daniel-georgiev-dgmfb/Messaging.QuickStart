using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Transport;
using Glasswall.Providers.Transport.AzureServiceBus.Exceptions;
using Microsoft.Azure.ServiceBus;

namespace Glasswall.Providers.Transport.AzureServiceBus.Transport
{
    [ExcludeFromCodeCoverage]
    public class ServiceBusTransport : ITransport
    {
        private readonly IGWLogger _logger;
        private QueueClient _queueClient;

        public ServiceBusTransport(Func<ITransportConfiguration> configuration, IGWLogger logger)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            this.Configuration = configuration();
            this._logger = logger;
        }

        public ITransportConfiguration Configuration { get; }
        public bool IsTransactional { get; }
        public int PendingMessages { get; }

        public Task Initialise(CancellationToken cancellationToken)
        {
            if (_queueClient != null)
                return Task.CompletedTask;

            var connectionString = new ServiceBusConnectionStringBuilder(Configuration.TransportConnection.ConnectionString);
            _queueClient = new QueueClient(connectionString);

            if (Configuration.TransportMode == Mode.SendReceive || Configuration.TransportMode == Mode.ReceiveOnly)
            {
                if (this.Configuration.Listeners.Count == 0)
                    throw new ArgumentException("No listeners registered in transport configuration.");
                RegisterOnMessageHandlerAndReceiveMessages();
            }

            return Task.CompletedTask;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            if (_queueClient != null)
                return;

            await Initialise(cancellationToken);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (_queueClient == null)
                return;

            await _queueClient.CloseAsync();
            this._queueClient = null;
        }

        public async Task Send(byte[] message, CancellationToken cancellationToken)
        {
            try
            {
                var BRMessage = new Message(message) { SessionId = Guid.NewGuid().ToString() };
                await _queueClient.SendAsync(BRMessage);
            }
            catch (Exception ex)
            {
                throw new MessageSendFailure($"Was unable to send Message to '{_queueClient.QueueName}'", ex);
            }
        }

        public Task Send(byte[] message, ITransaction transaction, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<byte[]>> ReadAllMessages(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CopyMessages(IEnumerable<byte[]> destination, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = this.Configuration.MaxDegreeOfParallelism,
                AutoComplete = false
            };
            
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                foreach (var x in Configuration.Listeners)
                    await x.ReceiveMessage(message.Body, token);
                if(this._queueClient.ReceiveMode == ReceiveMode.PeekLock)
                    await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
            }
            catch (Exception e)
            {
                this._logger.Log(LogLevel.Error, 0, String.Empty, e, (s, ex) => ex.ToString());
                throw;
            }
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}