using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnyLog;

namespace LF.Toolkit.UnitTests.NET45
{
    [TestClass]
    public class AnyLogTests
    {
        [TestMethod]
        public void TestConsoleLog()
        {
            LogFactory.Configurate("ConsoleLog");
            ILogFactory factory = LogFactory.Current;
            Assert.IsNotNull(factory);

            ILog log = factory.GetLog("DEBUG");
            Assert.IsNotNull(log);

            log.Debug("ConsoleLog output");
        }

        [TestMethod]
        public void TestLog4Net()
        {
            LogFactory.Configurate("Log4Net");
            ILogFactory factory = LogFactory.Current;
            Assert.IsNotNull(factory);

            ILog log = factory.GetLog("DEBUG");
            Assert.IsNotNull(log);

            log.Debug("Log4Net output");
        }
    }
}
