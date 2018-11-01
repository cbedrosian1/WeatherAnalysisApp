using System;

namespace WeatherDataAnalysis.Model
{
    /// <summary>
    ///     Stores information about a Day
    /// </summary>
    public class DailyStats
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the Date.
        /// </summary>
        /// <value>
        ///     The Date.
        /// </value>
        public DateTime Date { get; }

        /// <summary>
        ///     Gets or sets the low temperature.
        /// </summary>
        /// <value>
        ///     The low temperature.
        /// </value>
        public int LowTemperature { get; }

        /// <summary>
        ///     Gets or sets the high temperature.
        /// </summary>
        /// <value>
        ///     The high temperature.
        /// </value>
        public int HighTemperature { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DailyStats" /> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="lowTemperature">The low temperature.</param>
        /// <param name="highTemperature">The high temperature.</param>
        public DailyStats(DateTime date, int highTemperature, int lowTemperature)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }

            this.Date = date;
            this.LowTemperature = lowTemperature;
            this.HighTemperature = highTemperature;
        }

        #endregion
    }
}