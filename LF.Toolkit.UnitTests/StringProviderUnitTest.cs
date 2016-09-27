using LF.Toolkit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.UnitTests
{
    [TestClass]
    public class StringProviderUnitTest
    {
        [TestMethod]
        public void TestSplitToNumbers()
        {
            string value = "1,2,3,4,5";
            var arrays = StringProvider.SpiltToNumbers<int>(",", value);
            Assert.AreEqual(value, string.Join(",", arrays));
        }
    }
}
