using Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiverTest
{
	class TestMessageHandler : BaseMessageHandler<TestMessage>
    {
        public TestMessageHandler(IGWLogger<BaseMessageHandler<TestMessage>> logger) : base(logger)
        {
        }

        protected override async Task InvokeInternal(TestMessage message, CancellationToken cancellationToken)
        {
            base.Logger.Log(LogLevel.Information, 0, message.Name, null, (s, e) => s.ToString());
            await Task.Delay(5000);
        }
    }
}
