using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherDataAnalysis.Model
{
    /// <summary>
    ///     Takes a list of days and returns weather data about them
    /// </summary>
    public class WeatherData
    {
        #region Data members

        private const int YearPadding = 1000;
        private const int SizeFinder = 1;
        private int highestTemp;
        private int lowestTemp;

        private readonly ICollection<IGrouping<int, DailySummary>> conflictingDays;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the collection of days.
        /// </summary>
        /// <value>
        ///     The collection of days.
        /// </value>
        public ICollection<DailySummary> Days { get; set; }

        /// <summary>
        ///     Gets or sets the high temperature threshold.
        /// </summary>
        /// <value>
        ///     The high temperature threshold.
        /// </value>
        public int HighTemperatureThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the low temperature threshold.
        /// </summary>
        /// <value>
        ///     The low temperature threshold.
        /// </value>
        public int LowTemperatureThreshold { get; set; }

        /// <summary>
        ///     Gets the conflicting days count.
        /// </summary>
        /// <value>
        ///     The conflicting days count.
        /// </value>
        public int ConflictingDaysCount => this.conflictingDays.Count;

        /// <summary>
        ///     Gets or sets the size of each bucket
        /// </summary>
        /// <value>
        ///     The size of each bucket
        /// </value>
        public int HistogramBucketSize { get; set; }

        /// <summary>
        ///     Gets the range of numbers in each bucket
        /// </summary>
        /// <value>
        ///     The range of numbers in each bucket
        /// </value>
        public int HistogramRange => this.HistogramBucketSize - SizeFinder;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherData" /> class.
        ///     Precondition: days != null
        /// </summary>
        /// <param name="days">The days to take info from.</param>
        public WeatherData(ICollection<DailySummary> days)
        {
            this.Days = days ?? throw new ArgumentNullException(nameof(days));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherData" /> class.
        ///     Precondition: data != null AND days != null
        /// </summary>
        /// <param name="data">The existing data.</param>
        /// <param name="days">The new incoming data.</param>
        public WeatherData(WeatherData data, IEnumerable<DailySummary> days)
        {
            if (days == null)
            {
                throw new ArgumentNullException(nameof(days));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.Days = data.Days;
            var groupedDays = this.Days.Union(days)
                                  .GroupBy(day => day.Date.Year * YearPadding + day.Date.DayOfYear).ToList();
            this.conflictingDays = groupedDays.Where(group => group.Count() > 1).ToList();
            this.Days = this.Days.Union(groupedDays.Where(group => group.Count() == 1).Select(group => group.First()))
                            .ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the highest temperature of the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     List of days with the highest temperature of the year
        /// </returns>
        public List<DailySummary> FindHighestTemperatureDaysOfYear(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(day => day.HighTemperature)
                       .OrderByDescending(highTemp => highTemp.Key).First().ToList();
        }

        /// <summary>
        ///     Gets the lowest high temperature of the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     List of days with the lowest high temperature of the year
        /// </returns>
        public List<DailySummary> FindLowestHighTemperatureDaysOfYear(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(day => day.HighTemperature)
                       .OrderBy(highTemp => highTemp.Key).First()
                       .ToList();
        }

        /// <summary>
        ///     Gets the lowest temperature of the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     List of days with the lowest temperature of the year
        /// </returns>
        public List<DailySummary> FindLowestTemperatureDaysOfYear(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(day => day.LowTemperature)
                       .OrderBy(lowTemp => lowTemp.Key).First()
                       .ToList();
        }

        /// <summary>
        ///     Gets the highest low temperature of the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     List of days with the highest low temperature of the year
        /// </returns>
        public List<DailySummary> FindHighestLowTemperatureDaysOfYear(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(day => day.LowTemperature)
                       .OrderByDescending(lowTemp => lowTemp.Key).First()
                       .ToList();
        }

        /// <summary>
        ///     Gets the average temperature of the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="highOrLow">Whether the method is finding the high temp or low temp average</param>
        /// <returns>
        ///     Returns the average temperature of the year
        /// </returns>
        public double FindAverageTemperatureOfYear(int year, bool highOrLow)
        {
            double averageTemp;
            if (highOrLow)
            {
                averageTemp = this.Days.Where(day => day.Date.Year == year).Average(temp => temp.HighTemperature);
            }
            else
            {
                averageTemp = this.Days.Where(day => day.Date.Year == year).Average(temp => temp.LowTemperature);
            }

            return averageTemp;
        }

        /// <summary>
        ///     Gets the number of days over a specified temperature in a specified year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     Returns number of days over specified temperature
        /// </returns>
        public int FindNumberOfDaysOverTemperature(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).Count(days =>
                days.HighTemperature >= this.HighTemperatureThreshold ||
                days.LowTemperature >= this.HighTemperatureThreshold);
        }

        /// <summary>
        ///     Gets the number of days under a specified temperature in a specified year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     Returns the number of days under specified temperature
        /// </returns>
        public int FindNumberOfDaysUnderTemperature(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).Count(days =>
                days.LowTemperature <= this.LowTemperatureThreshold ||
                days.HighTemperature <= this.LowTemperatureThreshold);
        }

        /// <summary>
        ///     Groups the days by month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///     List of monthly summaries
        /// </returns>
        public ICollection<MonthlySummary> GroupDaysByMonth(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(month => month.Date.Month)
                       .Select(group => new MonthlySummary(group))
                       .ToList();
        }

        /// <summary>
        ///     Finds and returns a collection of years from the data
        /// </summary>
        /// <returns>Collection of years from the data</returns>
        public ICollection<int> FindYears()
        {
            var years = new List<int>();
            var yearGroups = this.Days.GroupBy(day => day.Date.Year);
            foreach (var currentYear in yearGroups)
            {
                years.Add(currentYear.First().Date.Year);
            }

            return years;
        }

        /// <summary>
        ///     Finds the next conflicting days.
        /// </summary>
        /// <returns>Collection of the next conflicting days</returns>
        public ICollection<DailySummary> FindNextConflictingDays()
        {
            if (this.ConflictingDaysCount > 0)
            {
                return this.conflictingDays.First().ToList();
            }

            return new List<DailySummary>();
        }

        /// <summary>
        ///     Merges the files based on the KeepOrReplace enum.
        /// </summary>
        /// <param name="action">The KeepOrReplace enum being chosen.</param>
        public void Merge(KeepOrReplace action)
        {
            var conDays = this.conflictingDays.First();
            if (action == KeepOrReplace.Replace)
            {
                foreach (var currentDay in conDays)
                {
                    if (!this.Days.Contains(currentDay))
                    {
                        this.Days.Add(currentDay);
                    }
                    else
                    {
                        this.Days.Remove(currentDay);
                    }
                }
            }

            this.conflictingDays.Remove(conDays);
        }

        /// <summary>
        ///     Generates a high or low temperature histogram.
        /// </summary>
        /// <param name="year">The year to generate the histogram for.</param>
        /// <param name="highOrLow">Whether the method is generating the high temp or low temp histogram</param>
        /// <returns>
        ///     A Dictionary of a range of temperatures and a number of days in that range
        /// </returns>
        public IDictionary<int, int> GenerateTempHistogram(int year, bool highOrLow)
        {
            if (highOrLow)
            {
                this.lowestTemp = this.FindLowestHighTemperatureDaysOfYear(year).First().HighTemperature;
                this.highestTemp = this.FindHighestTemperatureDaysOfYear(year).First().HighTemperature;
            }
            else
            {
                this.lowestTemp = this.FindLowestTemperatureDaysOfYear(year).First().LowTemperature;
                this.highestTemp = this.FindHighestLowTemperatureDaysOfYear(year).First().LowTemperature;
            }

            var topOfHistogram = this.findStartingTempForHistogram(this.highestTemp) + this.HistogramRange;
            var bottomOfRange = this.findStartingTempForHistogram(this.lowestTemp);

            return this.findNumbersInRange(year, highOrLow, topOfHistogram, bottomOfRange);
        }

        private int findStartingTempForHistogram(int temp)
        {
            var startingTemp = temp;
            while (startingTemp % this.HistogramBucketSize != 0)
            {
                startingTemp--;
            }

            return startingTemp;
        }

        private SortedDictionary<int, int> findNumbersInRange(int year, bool highOrLow, int topOfHistogram,
            int bottomOfRange)
        {
            var groupCounts = new SortedDictionary<int, int>();
            while (bottomOfRange <= topOfHistogram)
            {
                int numberInRange;
                var topOfRange = bottomOfRange + this.HistogramRange;
                if (highOrLow)
                {
                    numberInRange = this.Days.Where(day => day.Date.Year == year).Count(day =>
                        day.HighTemperature >= bottomOfRange &&
                        day.HighTemperature <= topOfRange);
                }
                else
                {
                    numberInRange = this.Days.Where(day => day.Date.Year == year).Count(day =>
                        day.LowTemperature >= bottomOfRange &&
                        day.LowTemperature <= topOfRange);
                }

                groupCounts.Add(bottomOfRange, numberInRange);
                bottomOfRange += this.HistogramBucketSize;
            }

            return groupCounts;
        }

        #endregion
    }
}