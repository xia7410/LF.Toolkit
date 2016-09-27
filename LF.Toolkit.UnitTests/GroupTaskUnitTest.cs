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
    public class GroupTaskUnitTest
    {
        IList<int> m_List = new List<int>();

        public GroupTaskUnitTest()
        {
            for (int i = 0; i < 100; i++)
            {
                m_List.Add(i);
            }
        }

        [TestMethod]
        public void TestWaitAsGroup()
        {
            //GroupTask.WaitAllAsGroup<int>((i) => Console.WriteLine(i), m_List, 10);
            GroupTask.WaitAllAsGroup<int>((i) => { Console.WriteLine(i); return Task.FromResult(0); }, m_List, 10);
        }
    }
}
