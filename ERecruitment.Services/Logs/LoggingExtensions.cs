using ERecruitment.Models.Enums;
using System;
using System.Threading;

namespace ERecruitment.Services.Logs
{
    public static class LoggingExtensions
    {
        public static void Debug(this ILogService logger, string message,
            Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Debug, message, exception);
        }

        public static void Information(this ILogService logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Information, message, exception);
        }

        public static void Warning(this ILogService logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Warning, message, exception);
        }

        public static void Error(this ILogService logger, string message, Exception exception = null)
        {
            FilteredLog(logger, LogLevel.Error, message, exception);
        }

        public static void Fatal(this ILogService logger, string message, Exception exception = null)
        {
            if (exception == null)
                FilteredLog(logger, LogLevel.Fatal, message, exception);
        }

        private static void FilteredLog(ILogService logger, LogLevel level, string message)
        {
            if (logger.IsEnabled(level))
            {
                string fullMessage = string.Empty;
                logger.InsertLog(level, message, fullMessage);
            }
        }

        private static void FilteredLog(ILogService logger, LogLevel level, string message, Exception exception = null)
        {
            //don't log thread abort exception
            if (exception is ThreadAbortException)
                return;

            if (logger.IsEnabled(level))
            {
                if (exception != null)
                {
                    logger.InsertLog(level, message, exception);
                }
                else
                {
                    logger.InsertLog(level, message);
                }
            }
        }
    }
}