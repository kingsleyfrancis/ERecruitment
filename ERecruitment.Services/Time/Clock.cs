using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using NodaTime.TimeZones;

namespace ERecruitment.Services.Time
{
    public class Clock : IClock
    {

        /// <summary>
        ///     Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
        /// </summary>
        /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
        /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
        public virtual TimeZoneInfo FindTimeZoneById(string id)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }

        /// <summary>
        ///     Returns a sorted collection of all the time zones
        /// </summary>
        /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
        public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        /// <summary>
        ///     Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt)
        {
            return ConvertToUserTime(dt, dt.Kind);
        }

        /// <summary>
        ///     Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents time that corresponds to 
        /// the dateTime parameter in customer time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            var currentUserTimeZoneInfo = TimeZoneInfo.Local;
            return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
        }

        /// <summary>
        ///     Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            TimeZoneInfo currentUserTimeZoneInfo = TimeZoneInfo.Local;
            return ConvertToUserTime(dt, sourceTimeZone, currentUserTimeZoneInfo);
        }

        /// <summary>
        ///     Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in customer time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone,
            TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
        }

        /// <summary>
        ///     Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>
        ///     A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime
        ///     parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.
        /// </returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt)
        {
            return ConvertToUtcTime(dt, dt.Kind);
        }

        /// <summary>
        ///     Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>
        ///     A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime
        ///     parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.
        /// </returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        /// <summary>
        ///     Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>
        ///     A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime
        ///     parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.
        /// </returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            if (sourceTimeZone.IsInvalidTime(dt))
            {
                //could not convert
                return dt;
            }

            return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
        }

        /// <summary>
        ///     Returns the first day in the week from the given date
        /// </summary>
        /// <param name="dayInWeek">The datetime specifying the date</param>
        /// <param name="cultureInfo">The culture info</param>
        /// <returns>The first day in the week.</returns>
        public DateTime GetFirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
                cultureInfo = CultureInfo.CurrentCulture;

            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
                firstDayInWeek = firstDayInWeek.AddDays(-1);

            return firstDayInWeek;
        }

        /// <summary>
        /// Returns the windows time zone id for a given country iso code.
        /// </summary>
        /// <param name="countryIsoCode">Country iso code.</param>
        /// <returns>Windows time zone id</returns>
        public string GetTimeZoneIdByTwoDigitCountryIso(string countryIsoCode)
        {
            if (string.IsNullOrWhiteSpace(countryIsoCode))
                return string.Empty;


            var source = TzdbDateTimeZoneSource.Default;
            IEnumerable<string> windowsZoneIds = source.ZoneLocations
                .Where(x => x.CountryCode.Equals(countryIsoCode, StringComparison.CurrentCultureIgnoreCase))

                .Select(tz => source.WindowsMapping.MapZones
                    .FirstOrDefault(x => x.TzdbIds.Contains(
                        source.CanonicalIdMap
                        .First(y => y.Value == tz.ZoneId).Key)))
                .Where(x => x != null)
                .Select(x => x.WindowsId)
                .Distinct();

            var zoneId = windowsZoneIds.FirstOrDefault();

            return zoneId;
        }

        public TimeZoneInfo GetTimeZoneInfoById(string timeZoneId)
        {
            if(string.IsNullOrWhiteSpace(timeZoneId))
                return null;

            var systemTimeZones = GetSystemTimeZones();

            if (systemTimeZones.Any())
            {
                var zone = systemTimeZones
                    .Where(a => a.Id.Equals(timeZoneId,
                        StringComparison.InvariantCultureIgnoreCase))
                    .Select(a => a)
                    .FirstOrDefault();

                return zone;
            }
            return null;
        }

        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public DateTime GetCurrentDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}