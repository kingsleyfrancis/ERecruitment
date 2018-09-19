using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERecruitment.Core.Common;
using ERecruitment.Models.Enums;
using ERecruitment.Models.Logs;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Patterns.UnitOfWork;
using ERecruitment.Services.Events;
using ERecruitment.Services.Time;

namespace ERecruitment.Services.Logs
{
    /// <summary>
    ///     Default logger
    /// </summary>
    public class LogService : Service<Log>, ILogService
    {
        #region Fields

        private readonly IClock _clock;
        private readonly IEventPublisher _eventPublisher;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="webHelper">Web helper</param>
        /// <param name="unitOfWork">Unit of work helper</param>
        /// <param name="clock">Common application date/time interface</param>
        public LogService(IWebHelper webHelper, IUnitOfWorkAsync
            unitOfWork, IClock clock, IEventPublisher eventPublisher)
            : base(unitOfWork.RepositoryAsync<Log>())
        {
            _webHelper = webHelper;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utitilities

        #endregion

        #region Methods

        /// <summary>
        ///     Determines whether a log level is enabled
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Result</returns>
        public virtual bool IsEnabled(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        ///     Logs an exception
        /// </summary>
        /// <param name="exception">exception to log</param>
        public void LogException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception",
                    "The exception provided cannot be null.");

            var log = new Log
            {
                CreatedOnUtc = _clock.GetCurrentDateTimeUtc(),
                FullMessage = exception.ToString(),
                ShortMessage = exception.Message,
                LogLevel = LogLevel.Error,
                ObjectState = ObjectState.Added,
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                StackTrace = exception.StackTrace,
                Source = exception.Source
            };

            Exception innerExp = exception.InnerException;
            do
            {
                if (innerExp == null) break;

                log.FullMessage += " /n/r/n/r " + innerExp.Message;
                log.StackTrace += " /n/r/n/r " + innerExp.StackTrace;
                log.Source += " /n/r/n/r " + innerExp.Source;


                innerExp = innerExp.InnerException;
            } while (innerExp != null);


            Insert(log);
            _unitOfWork.SaveChanges();

            _eventPublisher.EntityInserted(log);
        }

        /// <summary>
        ///     Deletes a log item
        /// </summary>
        /// <param name="log">Log item</param>
        public virtual void DeleteLog(Log log)
        {
            if (log == null)
                throw new ArgumentNullException("log");

            Delete(log);
            _unitOfWork.SaveChanges();
            _eventPublisher.EntityDeleted(log);
        }

        /// <summary>
        ///     Clears a log
        /// </summary>
        public virtual Task ClearLog(LogDept dept = LogDept.All)
        {
            //Todo: Needs improvement, will use stored procedure so as to avoid performance hit

            Task task = Task.Run(() =>
            {
                switch (dept)
                {
                    case LogDept.Error:
                    {
                        List<Log> errors = Query(a => a.LogLevel == LogLevel.Error)
                            .Select().ToList();

                        if (errors.Any())
                        {
                            foreach (Log error in errors)
                            {
                                Log err = error;
                                err.ObjectState = ObjectState.Deleted;
                                Delete(err);
                            }
                        }
                        break;
                    }
                    case LogDept.Fatal:
                    {
                        List<Log> fatals = Query(a => a.LogLevel == LogLevel.Fatal)
                            .Select().ToList();

                        if (fatals.Any())
                        {
                            foreach (Log fatal in fatals)
                            {
                                Log fa = fatal;
                                fa.ObjectState = ObjectState.Deleted;

                                Delete(fa);
                            }
                        }
                        break;
                    }
                    case LogDept.Information:
                    {
                        List<Log> infos = Query(a => a.LogLevel == LogLevel.Fatal)
                            .Select().ToList();

                        if (infos.Any())
                        {
                            foreach (Log info in infos)
                            {
                                Log msg = info;
                                msg.ObjectState = ObjectState.Deleted;

                                Delete(msg);
                            }
                        }
                        break;
                    }
                    case LogDept.Warning:
                    {
                        List<Log> warnings = Query(a => a.LogLevel == LogLevel.Fatal)
                            .Select().ToList();

                        if (warnings.Any())
                        {
                            foreach (Log warning in warnings)
                            {
                                Log warn = warning;
                                warn.ObjectState = ObjectState.Deleted;

                                Delete(warn);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        List<Log> allLogs = Query().Select().ToList();

                        foreach (Log log in allLogs)
                        {
                            log.ObjectState = ObjectState.Deleted;
                            Delete(log);
                        }
                        break;
                    }
                }
                _unitOfWork.SaveChanges();
            });
            return task;
        }


        /// <summary>
        ///     Gets all log items
        /// </summary>
        /// <param name="fromUtc">Log item creation from; null to load all records</param>
        /// <param name="toUtc">Log item creation to; null to load all records</param>
        /// <param name="message">Message</param>
        /// <param name="logLevel">Log level; null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count to return</param>
        /// <returns>Log item collection</returns>
        public virtual List<Log> GetAllLogs(DateTime? fromUtc, DateTime? toUtc,
            string message, LogLevel? logLevel, int pageIndex, int pageSize, out int totalCount)
        {
            IQueryable<Log> logQuery = Queryable();
            logQuery = logQuery.Where(a => a.IsResolved == false);
            if (fromUtc.HasValue)
            {
                logQuery = logQuery.Where(a => a.CreatedOnUtc >= fromUtc.Value);
            }

            if (toUtc.HasValue)
            {
                logQuery = logQuery.Where(a => a.CreatedOnUtc <= toUtc.Value);
            }

            if (logLevel.HasValue)
                logQuery = logQuery.Where(a => a.LogLevel == logLevel.Value);

            if (!String.IsNullOrWhiteSpace(message))
                logQuery = logQuery.Where(a => a.FullMessage.Contains(message) ||
                                               a.ShortMessage.Contains(message));

            totalCount = logQuery.Count();

            IOrderedQueryable<Log> list = logQuery.Skip(pageSize*pageIndex)
                .Take(pageSize)
                .OrderBy(a => a.CreatedOnUtc);

            return list.ToList();
        }

        public List<Log> GetAllLogs(bool getResolved = false)
        {
            List<Log> logs = Query(a => a.IsResolved == getResolved).Select().ToList();
            return logs;
        }

        /// <summary>
        ///     Gets a log item
        /// </summary>
        /// <param name="logId">Log item identifier</param>
        /// <returns>Log item</returns>
        public virtual Log GetLogById(int logId)
        {
            if (logId == 0)
                return null;

            Log log = Query(a => a.Id == logId)
                .Select()
                .FirstOrDefault();

            return log;
        }

        /// <summary>
        ///     Get log items by identifiers
        /// </summary>
        /// <param name="logIds">Log item identifiers</param>
        /// <returns>Log items</returns>
        public virtual IList<Log> GetLogByIds(int[] logIds)
        {
            if (logIds == null || logIds.Length == 0)
                return new List<Log>();

            IEnumerable<Log> query = from l in Query()
                .Select()
                where logIds.Contains(l.Id)
                select l;

            List<Log> logItems = query.ToList();
            //sort by passed identifiers

            return logIds.Select(id => logItems
                .Find(x => x.Id == id))
                .Where(log => log != null).ToList();
        }


        /// <summary>
        ///     Inserts a log item
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="shortMessage">The short message</param>
        /// <param name="fullMessage">The full message</param>
        /// <param name="profile">The profile to associate log record with</param>
        /// <returns>A log item</returns>
        public virtual Log InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "")
        {
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = _clock.GetCurrentDateTimeUtc()
            };

            Insert(log);
            _unitOfWork.SaveChanges();

            _eventPublisher.EntityInserted(log);

            return log;
        }

        public Log InsertLog(LogLevel logLevel, string shortMessage, Exception exception)
        {
            var log = new Log
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                IpAddress = _webHelper.GetCurrentIpAddress(),
                PageUrl = _webHelper.GetThisPageUrl(true),
                ReferrerUrl = _webHelper.GetUrlReferrer(),
                CreatedOnUtc = _clock.GetCurrentDateTimeUtc()
            };


            Insert(log);
            _unitOfWork.SaveChanges();


            _eventPublisher.EntityInserted(log);
            return log;
        }


        public bool MarkAsResolved(int id)
        {
            if (CommonHelper.IsWithinIntegerRange(id))
            {
                Log log = Find(id);

                if (log != null)
                {
                    log.IsResolved = true;
                    log.ObjectState = ObjectState.Modified;
                    Update(log);
                    _unitOfWork.SaveChanges();

                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteRelated(int id)
        {
            if (CommonHelper.IsWithinIntegerRange(id))
            {
                Log log = Find(id);

                if (log == null)
                    return false;

                var logList = new List<Log>();

                if (log.LogLevel == LogLevel.Information)
                {
                    List<Log> logs = Query(a => a.LogLevel == LogLevel.Information)
                        .Select().ToList();

                    logList = logs;
                }
                else if (log.LogLevel == LogLevel.Debug)
                {
                    List<Log> logs = Query(a => a.LogLevel == LogLevel.Debug)
                        .Select().ToList();

                    logList = logs;
                }
                else
                {
                    IQueryable<Log> logQuery = Queryable();

                    //Check if the short message are the same.
                    if (!string.IsNullOrWhiteSpace(log.ShortMessage))
                    {
                        logQuery =
                            logQuery.Where(
                                a => a.ShortMessage
                                    .Contains(log.ShortMessage));
                    }


                    //Check if the long message are the same
                    if (!string.IsNullOrWhiteSpace(log.FullMessage))
                    {
                        logQuery =
                            logQuery.Where(
                                a => a.FullMessage
                                    .Equals(log.FullMessage, StringComparison.InvariantCultureIgnoreCase));
                    }

                    logList = logQuery.ToList();
                }


                foreach (Log lg in logList)
                {
                    lg.ObjectState = ObjectState.Deleted;
                    Delete(lg);
                    await _unitOfWork.SaveChangesAsync();
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}