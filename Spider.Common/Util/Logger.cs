using System;
using System.Configuration;
using log4net;
using log4net.Config;

namespace Spider.Util
{
    public static class Logger
    {
        private static ILog log;

        static Logger()
        {
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(ConfigurationManager.AppSettings["loggerName"]);
        }
        public static void Debug(object message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.Debug(message, exception);
        }

        public static void Debug(object message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.Debug(message);
        }

        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.DebugFormat(provider, format, args);
        }

        public static void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.DebugFormat(format, arg0, arg1, arg2);
        }

        public static void DebugFormat(string format, object arg0, object arg1)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.DebugFormat(format, arg0, arg1);
        }

        public static void DebugFormat(string format, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.DebugFormat(format, arg0);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            log.DebugFormat(format, args);
        }

        public static void Error(object message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.Error(message, exception);
        }

        public static void Error(object message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.Error(message);
        }

        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.ErrorFormat(provider,format,args);
        }

        public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.ErrorFormat(format,arg0,arg1,arg2);
        }

        public static void ErrorFormat(string format, object arg0, object arg1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.ErrorFormat(format, arg0, arg1);
        }

        public static void ErrorFormat(string format, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.ErrorFormat(format, arg0);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            log.ErrorFormat(format, args);
        }

        public static void Fatal(object message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.Fatal(message, exception);
        }

        public static void Fatal(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.Fatal(message);
        }

        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.FatalFormat(provider, format, args);
        }

        public static void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.FatalFormat(format,  arg0,  arg1,  arg2);
        }

        public static void FatalFormat(string format, object arg0, object arg1)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.FatalFormat(format, arg0, arg1);
        }

        public static void FatalFormat(string format, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.FatalFormat(format, arg0);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            log.FatalFormat(format, args);
        }

        public static void Info(object message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.Info(message,exception);
        }

        public static void Info(object message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.Info(message);
        }

        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.InfoFormat(provider, format, args);
        }

        public static void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.InfoFormat(format, arg0, arg1, arg2);
        }

        public static void InfoFormat(string format, object arg0, object arg1)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.InfoFormat(format, arg0, arg1);
        }

        public static void InfoFormat(string format, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.InfoFormat(format, arg0);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            log.InfoFormat(format, args);
        }

        public static void Warn(object message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.Warn(message, exception);
        }

        public static void Warn(object message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.Warn(message);
        }

        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.WarnFormat( provider,  format, args);
        }

        public static void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.WarnFormat(format, arg0, arg1, arg2);
        }

        public static void WarnFormat(string format, object arg0, object arg1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.WarnFormat(format, arg0, arg1);
        }

        public static void WarnFormat(string format, object arg0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.WarnFormat(format, arg0);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            log.WarnFormat(format, args);
        }
    }
}
