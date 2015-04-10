using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Util.Log
{
    public sealed class Log4NetProvider
    {
        static readonly ILog ERRORLOG;
        static readonly ILog DEBUGLOG;

        static Log4NetProvider()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.GetFullPath("log4net.config")));  
            ERRORLOG = LogManager.GetLogger("errorAppender");
            DEBUGLOG = LogManager.GetLogger("debugAppender");
        }

        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }

        public static void Error(object message)
        {
            if (ERRORLOG != null)
            {
                ERRORLOG.Error(message);
            }
        }

        public static void Error(object message, Exception e)
        {
            if (ERRORLOG != null)
            {
                ERRORLOG.Error(message, e);
            }
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            if (ERRORLOG != null)
            {
                ERRORLOG.ErrorFormat(format, args);
            }
        }

        public static void Debug(object message)
        {
            if (DEBUGLOG != null)
            {
                DEBUGLOG.Debug(message);
            }
        }

        public static void Debug(object message, Exception e)
        {
            if (DEBUGLOG != null)
            {
                DEBUGLOG.Debug(message, e);
            }
        }

        public static void DebugFormat(string format, params object[] args)
        {
            if (DEBUGLOG != null)
            {
                DEBUGLOG.DebugFormat(format, args);
            }
        }
    }
}
