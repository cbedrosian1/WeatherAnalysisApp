using System;

namespace WeatherDataAnalysis.Utility
{
    /// <summary>
    ///     Extends the date time class in order to format the ending for a day
    /// </summary>
    public static class DateTimeSuffixFormatter
    {
        #region Methods

        /// <summary>
        ///     Formats the ending for day.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns>the proper ending</returns>
        public static string FormatEndingForDay(DateTime day)
        {
            switch (day.Date.Day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }

        #endregion
    }
}