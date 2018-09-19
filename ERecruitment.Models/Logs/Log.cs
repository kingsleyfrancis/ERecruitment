using System;
using System.ComponentModel.DataAnnotations.Schema;
using ERecruitment.Patterns.Infrastructure;
using ERecruitment.Models.Enums;

namespace ERecruitment.Models.Logs
{
    public class Log : IEntity
    {
        public int LogLevelId { get; set; }

        /// <summary>
        ///     Gets or sets the short message
        /// </summary>
        public string ShortMessage { get; set; }

        /// <summary>
        ///     Gets or sets the full exception
        /// </summary>
        public string FullMessage { get; set; }

        /// <summary>
        ///     Gets or sets the IP address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///     Gets or sets the page URL
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        ///     Gets or sets the referrer URL
        /// </summary>
        public string ReferrerUrl { get; set; }

        /// <summary>
        ///     Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        ///     Gets or set the stack trace of the exception if avaliable
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///     Gets or sets the log level
        /// </summary>
        public LogLevel LogLevel
        {
            get { return (LogLevel) LogLevelId; }
            set { LogLevelId = (int) value; }
        }

        /// <summary>
        ///     Gets or sets the source of the exception.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Identifies whether the log has been resolved
        /// </summary>
        public bool IsResolved { get; set; }

        public bool IsSeen { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public int Id { get; set; }

        public byte[] TimeStamp { get; set; }
    }
}