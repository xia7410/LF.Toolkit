using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Data.Dapper
{
    /// <summary>
    /// 表示Sql存储简单事件通知类
    /// </summary>
    public static class SqlStorageEvents
    {
        static IDictionary<Type, object> m_Events = new Dictionary<Type, object>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        public static void Register<T>(Action<object, T> target)
            where T : EventArgs
        {
            if (!m_Events.TryGetValue(typeof(T), out object handler))
            {
                //创建一个空的事件处理
                handler = new EventHandler<T>((sender, args) => { });
            }
            EventHandler<T> eventHandler = handler as EventHandler<T>;
            eventHandler += new EventHandler<T>(target);
            m_Events[typeof(T)] = eventHandler;
        }

        /// <summary>
        /// 发出事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Emit<T>(object sender, T args)
            where T : EventArgs
        {
            if (m_Events.TryGetValue(typeof(T), out object handler))
            {
                (handler as EventHandler<T>)?.Invoke(sender, args);
            }
        }
    }
}
