using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.Kernel.Logging;
using Glasswall.Kernel.Resilience;
using Glasswall.Kernel.Transport;
using Glasswall.Providers.Transport.AzureServiceBus.Exceptions;

namespace Glasswall.Providers.Transport.AzureServiceBus.Transport
{
    public class ServiceBusTransportManager : ITransportManager
    {
        private readonly ITransport _transport;
        private readonly ICircuitBreaker _circuitBreaker;
        private readonly IGWLogger _eventLogger;

        internal ITransport Transport { get { return this._transport; } }

        public ServiceBusTransportManager(ITransport transport, ICircuitBreaker circuitBreaker, IGWLogger eventLogger)
        {
            _transport = transport ?? throw new ArgumentNullException(nameof(transport));
            _circuitBreaker = circuitBreaker ?? throw new ArgumentNullException(nameof(circuitBreaker));
            _eventLogger = eventLogger ?? throw new ArgumentNullException(nameof(eventLogger));

            _circuitBreaker.CircuitBreakerOpened += CircuitBreakerOnCircuitBreakerOpened;
            _circuitBreaker.CircuitBreakerReset += CircuitBreakerOnCircuitBreakerReset;
        }

        private void CircuitBreakerOnCircuitBreakerOpened(object sender, CircuitBreakerOpenedEventArgs args)
        {
            var queueName = _transport.Configuration.TransportConnection.QueueName;
            _eventLogger.Log(LogLevel.Information, (int)Logging.EventId.CircuitBreakerOpened, 
                $"Circuit Breaker opened on queue '{queueName}' for duration of {args.BreakDuration} due to {args.TriggerException.Message}", null, (_, __) => String.Format("{0}, {1}", _, __.ToString()));
        }

        private void CircuitBreakerOnCircuitBreakerReset(object sender, EventArgs args)
        {
            var queueName = _transport.Configuration.TransportConnection.QueueName;

            _eventLogger.Log(LogLevel.Information, (int)Logging.EventId.CircuitBreakerReset, $"Circuit Breaker on queue '{queueName}' reset", null, (_, __) => _);
        }

        public Task Initialise(CancellationToken cancellationToken)
        {
            _transport.Initialise(cancellationToken);
            return Task.CompletedTask;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            await _transport.Start(cancellationToken);
            _eventLogger.Log(LogLevel.Information, 0, "ServiceBusTransportManager started", null, (_, __) => _);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            await _transport.Stop(cancellationToken);
            _eventLogger.Log(LogLevel.Information, 0, "ServiceBusTransportManager stopped", null, (_, __) => _);
        }

        public async Task EnqueueMessage(byte[] message, CancellationToken cancellationToken)
        {
            await _circuitBreaker.ExecuteAsync(
                executeAction: async c => await _transport.Send(message, c),
                failureAction: e => throw new MessageSendFailure("Message queuing failure due to Circuit Breaker", e),
                cancellationToken: cancellationToken);
        }

        public Task RegisterListener(IMessageListener listener)
        {
            _transport.Configuration.Listeners.Add(listener);
            return Task.CompletedTask;
        }
    }
}