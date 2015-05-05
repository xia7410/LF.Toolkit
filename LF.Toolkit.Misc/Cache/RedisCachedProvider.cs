using LF.Toolkit.Util.Log;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LF.Toolkit.Misc.Cache
{
    public class RedisCachedProvider
    {
        static object mutex = new object();
        static ConnectionMultiplexer multiplexer;

        public static void Connect()
        {
            if (multiplexer == null || !multiplexer.IsConnected)
            {
                string configuration = ConfigurationManager.AppSettings["redisConfiguration"];
                if (string.IsNullOrEmpty(configuration)) throw new ArgumentNullException("configuration");

                lock (mutex)
                {
                    if (multiplexer == null)
                    {
                        multiplexer = ConnectionMultiplexer.Connect(configuration);
                        multiplexer.ConnectionFailed += (sender, e) => { Log4NetProvider.Error(e.Exception); };
                    }
                }
            }
        }

        public static IDatabase GetDatabase(int db = 0, object asyncState = null)
        {
            Connect();

            return multiplexer.GetDatabase(db, asyncState);
        }

        public static IServer GetServer(string hostAndPort, object asyncState = null)
        {
            Connect();

            return multiplexer.GetServer(hostAndPort, asyncState);
        }
    }
}