using System;
using Glasswall.Kernel.Logging;
using Logging.QuickStart;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;
using NUnit.Framework;

namespace LoggingTests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void Test1()
        {
            //ARRANGE
            Microsoft.Extensions.Logging.LogLevel logLevelActual = Microsoft.Extensions.Logging.LogLevel.None;
            Microsoft.Extensions.Logging.EventId eventActual = 0;
            object stateActual = null;
            Exception exceptionActual = null;
            Func<object, Exception, string> formaterActual = null;
            var loggerProviderMock = new Mock<ILoggerProvider>();
            var loggerMock = new Mock<ILogger>();
            loggerMock.Setup(x => x.IsEnabled(It.IsAny<Microsoft.Extensions.Logging.LogLevel>()))
                .Returns(true);
            loggerMock.Setup(x => x.Log(It.IsAny<Microsoft.Extensions.Logging.LogLevel>(), It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()))
                .Callback(new Action<Microsoft.Extensions.Logging.LogLevel, Microsoft.Extensions.Logging.EventId, object, Exception, Func<object, Exception, string>>((l, ev, s, ex, f) =>
                {
                    logLevelActual = l;
                    eventActual = ev;
                    stateActual = s;
                    exceptionActual = ex;
                    formaterActual = f;
                }));
            loggerProviderMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(loggerMock.Object);
            var loggerFactory = new LoggerFactory(new[] { loggerProviderMock.Object });
            var logger = new Glasswall.Providers.Logging.Microsoft.Logger<Service>(loggerFactory);
            var servicetoTest = new Service(logger);
            //ACT
            servicetoTest.LogSomething("Message to Test");
            var stateType = stateActual.GetType();
            var output = formaterActual(stateActual, exceptionActual);
            //ASSERT
            Assert.AreEqual(logLevelActual, Microsoft.Extensions.Logging.LogLevel.Information);
            Assert.AreEqual(eventActual, new Microsoft.Extensions.Logging.EventId(0));
            Assert.AreEqual(typeof(FormattedLogValues), stateType);
            Assert.AreEqual("Information message with parameters. Parameter: 'Message to Test'", output);
        }
    }
}
