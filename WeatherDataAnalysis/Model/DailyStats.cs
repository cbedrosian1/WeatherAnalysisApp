using System;

namespace WeatherDataAnalysis.Model
{
    /// <summary>
    ///     Stores information about a Day
    /// </summary>
    ///
    [Serializable]
    public class DailyStats
    {
       
        #region Properties

        /// <summary>
        ///     Gets the Date.
        /// </summary>
        /// <value>
        ///     The Date.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Gets the low temperature.
        /// </summary>
        /// <value>
        ///     The low temperature.
        /// </value>
        public int LowTemperature { get; set; }

        /// <summary>
        ///     Gets the high temperature.
        /// </summary>
        /// <value>
        ///     The high temperature.
        /// </value>
        public int HighTemperature { get; set; }

        /// <summary>
        /// Gets the precipitation for the day
        /// </summary>
        /// <value>the precipitation</value>
        public double Precipitation { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DailyStats" /> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="lowTemperature">The low temperature.</param>
        /// <param name="highTemperature">The high temperature.</param>
        /// <param name="precipitation"> the precipitation of the day</param>
        public DailyStats(DateTime date, int highTemperature, int lowTemperature, double precipitation)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }

            this.Date = date;
            this.LowTemperature = lowTemperature;
            this.HighTemperature = highTemperature;
            this.Precipitation = precipitation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DailyStats"/> class.
        /// </summary>
        public DailyStats()
        {
            //TODO
        }

        #endregion
    }
}