using System;

namespace Logging.QuickStart
{
    public class TransactionalState
    {
        public Guid TransactionId { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public TransactionalState(Guid transactionId, string message, Exception exception)
        {
            TransactionId = transactionId;
            Message = message;
            Exception = exception;
        }

        public override string ToString()
        {
            return $"{TransactionId} | {Message} | {Exception}";
        }

        public static string MessageFormat(object state, Exception error)
        {
            var transactionalState = state as TransactionalState;
            return transactionalState?.ToString();
        }
    }
}