using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace DotNetMonitor.Common
{
    public static class LogHelper
    {
        static LogHelper()
        {
            var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
        }
        public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Logger.Debug(message);
                    break;

                case LogLevel.Info:
                    Logger.Info(message);
                    break;

                case LogLevel.Warn:
                    Logger.Warn(message);
                    break;

                case LogLevel.Error:
                    Logger.Error(message);
                    break;
            }
        }
    }
}