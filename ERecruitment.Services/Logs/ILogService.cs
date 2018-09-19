using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Logs;

namespace ERecruitment.Services.Logs
{
    public interface ILogService : IService<Log>
    {
        void LogException(Exception exception);

        /// <summary>
        ///     Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        ///     Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        void DeleteLog(Log log);

        /// <summary>
        ///     Clears a log
        /// </summary>
        Task ClearLog(LogDept dept = LogDept.All);

        /// <summary>
        ///     Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">The total count of all the items in the database</param>
        /// <returns>Log item collection</returns>
        List<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize, out int totalCount);


        List<Log> GetAllLogs(bool getResolved = false);

        /// <summary>
        ///     Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        Log GetLogById(int logId);

        /// <summary>
        ///     Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        IList<Log> GetLogByIds(int[] logIds);

        /// <summary>
        ///     Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="profile">The profile in which the error occured</param>
        /// <returns>A log item</returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "");

        /// <summary>
        ///     Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="exception">The exception that occured</param>
        /// <param name="profile">The profile in which the error occured</param>
        /// <returns>A log item</returns>
        Log InsertLog(LogLevel logLevel, string shortMessage, Exception exception);



        /// <summary>
        ///     Marks a log message as resolved
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>A boolean showing whether the log has been resolved</returns>
        bool MarkAsResolved(int id);


        /// <summary>
        ///     Deletes the logs and related items
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>A boolean showing whether the log has been deleted</returns>
        Task<bool> DeleteRelated(int id);
    }
}