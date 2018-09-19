namespace ERecruitment.Models.Enums
{
    /// <summary>
    ///     Represents a log level
    /// </summary>
    public enum LogLevel
    {
        Debug = 10,
        Information = 20,
        Warning = 30,
        Error = 40,
        Fatal = 50
    }

    /// <summary>
    ///     Userd to control the dept of logs to clear
    /// </summary>
    public enum LogDept
    {
        All,
        Information,
        Warning,
        Error,
        Fatal
    }
}