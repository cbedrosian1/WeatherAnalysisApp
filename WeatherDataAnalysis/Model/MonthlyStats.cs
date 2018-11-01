using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherDataAnalysis.Model
{
    /// <summary>
    ///     Stores information about months in the year
    /// </summary>
    public class MonthlyStats
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the month number.
        /// </summary>
        /// <value>
        ///     The month number.
        /// </value>
        public int MonthNumber { get; }

        /// <summary>
        ///     Gets or sets days of the month sorted by high temperature.
        /// </summary>
        /// <value>
        ///     Days of the month sorted by high temperature.
        /// </value>
        public List<DailyStats> HighTempDays { get; }

        /// <summary>
        ///     Gets or sets days of the month sorted by low temperature.
        /// </summary>
        /// <value>
        ///     Days of the month sorted by high temperature..
        /// </value>
        public List<DailyStats> LowTempDays { get; }

        /// <summary>
        ///     Gets or sets the average high temperature of a month
        /// </summary>
        /// <value>
        ///     The average high temperature of a month.
        /// </value>
        public double AverageHighTemperature { get; }

        /// <summary>
        ///     Gets or sets the average low temperature of a month.
        /// </summary>
        /// <value>
        ///     The average low temperature of a month.
        /// </value>
        public double AverageLowTemperature { get; }

        /// <summary>
        ///     Gets or sets the days in a month.
        /// </summary>
        /// <value>
        ///     The days in a month.
        /// </value>
        public int DaysInMonth { get; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MonthlyStats" /> class.
        ///     Precondition: monthDays != null
        /// </summary>
        /// <param name="monthDays">The month days.</param>
        public MonthlyStats(IGrouping<int, DailyStats> monthDays)
        {
            if (monthDays == null)
            {
                throw new ArgumentNullException(nameof(monthDays));
            }

            this.HighTempDays = monthDays.GroupBy(temp => temp.HighTemperature).OrderByDescending(temp => temp.Key)
                                         .First().OrderBy(day => day.Date.Day).ToList();
            this.LowTempDays = monthDays.GroupBy(temp => temp.LowTemperature).OrderBy(temp => temp.Key)
                                        .First().OrderBy(day => day.Date.Day).ToList();
            this.AverageHighTemperature = monthDays.ToList().Average(temp => temp.HighTemperature);
            this.AverageLowTemperature = monthDays.ToList().Average(temp => temp.LowTemperature);
            this.DaysInMonth = monthDays.ToList().Count;
            this.MonthNumber = monthDays.Key;
        }

        #endregion
    }
}