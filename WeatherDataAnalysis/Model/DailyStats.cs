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
        ///     Gets the Date.
        /// </summary>
        /// <value>
        ///     The Date.
        /// </value>
        public DateTime Date { get; }

        /// <summary>
        ///     Gets the low temperature.
        /// </summary>
        /// <value>
        ///     The low temperature.
        /// </value>
        public int LowTemperature { get; }

        /// <summary>
        ///     Gets the high temperature.
        /// </summary>
        /// <value>
        ///     The high temperature.
        /// </value>
        public int HighTemperature { get; }

        /// <summary>
        /// Gets the percipitation for the day
        /// </summary>
        /// <value>the percipitation</value>
        public double Percipitation { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DailyStats" /> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="lowTemperature">The low temperature.</param>
        /// <param name="highTemperature">The high temperature.</param>
        /// <param name="percipitation"> the percipitation of the day</param>
        public DailyStats(DateTime date, int highTemperature, int lowTemperature, double percipitation)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }

            this.Date = date;
            this.LowTemperature = lowTemperature;
            this.HighTemperature = highTemperature;
            this.Percipitation = percipitation;
        }

        #endregion
    }
}