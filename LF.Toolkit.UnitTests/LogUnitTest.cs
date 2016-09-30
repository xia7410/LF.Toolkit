using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Reflection;

namespace LF.Toolkit.UnitTests
{
    [TestClass]
    public class LogUnitTest
    {
        [TestMethod]
        public void TestLog()
        {
            var logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName + ".Info");
            logger.Info("info file");
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName + ".Debug");
            logger.Debug("debug file");
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName + ".Error");
            logger.Error("error file");
            logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.FullName + ".ColoredConsole");
            logger.Info("nlog console");
        }

    }
}
