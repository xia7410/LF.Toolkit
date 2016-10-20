using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Common
{
    public static class GroupTask
    {
        public static void WaitAllAsGroup<T>(Func<T, Task> func, IEnumerable<T> collection, int groupCount)
        {
            int total = collection.Count();
            if (total > 0)
            {
                //共分成几组
                int groups = total / groupCount;
                if (total % groupCount != 0)
                {
                    groups += 1;
                }
                //分组等待任务执行完毕
                for (int i = 0; i < groups; i++)
                {
                    var list = new List<Task>();
                    foreach (var item in collection.Skip(i * groupCount).Take(groupCount))
                    {
                        list.Add(Task.Run(() => func(item)));
                    }

                    if (list.Count > 0)
                    {
                        Task.WaitAll(list.ToArray());
                    }
                }
            }
        }

        public static void WaitAllAsGroup<T>(Action<T> action, IEnumerable<T> collection, int groupCount)
        {
            int total = collection.Count();
            if (total > 0)
            {
                //共分成几组
                int groups = total / groupCount;
                if (total % groupCount != 0)
                {
                    groups += 1;
                }
                //分组等待任务执行完毕
                for (int i = 0; i < groups; i++)
                {
                    var list = new List<Task>();
                    foreach (var item in collection.Skip(i * groupCount).Take(groupCount))
                    {
                        list.Add(Task.Run(() => action(item)));
                    }

                    if (list.Count > 0)
                    {
                        Task.WaitAll(list.ToArray());
                    }
                }
            }
        }

        public static async Task<IEnumerable<U>> WhenAllAsGroup<T, U>(Func<T, Task<U>> func, IEnumerable<T> collection, int groupCount)
        {
            var results = new List<U>();
            int total = collection.Count();
            if (total > 0)
            {
                //共分成几组
                int groups = total / groupCount;
                if (total % groupCount != 0)
                {
                    groups += 1;
                }
                //分组等待任务执行完毕
                for (int i = 0; i < groups; i++)
                {
                    var list = new List<Task<U>>();
                    foreach (var item in collection.Skip(i * groupCount).Take(groupCount))
                    {
                        list.Add(Task.Run(() => func(item)));
                    }

                    if (list.Count > 0)
                    {
                        var ls = await Task.WhenAll(list);
                        results.AddRange(ls);
                    }
                }
            }

            return results;
        }
    }
}
