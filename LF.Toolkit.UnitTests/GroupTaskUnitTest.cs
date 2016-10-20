using LF.Toolkit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        Task<string> Get(string url)
        {
            var client = new HttpClient();
            return client.GetStringAsync(url);
        }

        [TestMethod]
        public void TestWaitAsGroup()
        {
            //GroupTask.WaitAllAsGroup<int>((i) => Console.WriteLine(i), m_List, 10);
            GroupTask.WaitAllAsGroup<int>((i) => { Console.WriteLine(i); return Task.FromResult(0); }, m_List, 10);
        }

        [TestMethod]
        public async Task TestWhenAllAsGroup()
        {
            var urls = new string[]
            {
                "https://www.baidu.com",
                "http://cn.bing.com/",
                "https://wx.qq.com/",
                "https://translate.google.cn/"
            };
            var list = await GroupTask.WhenAllAsGroup<string, string>((i) => Get(i), urls, 2);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }
    }
}
