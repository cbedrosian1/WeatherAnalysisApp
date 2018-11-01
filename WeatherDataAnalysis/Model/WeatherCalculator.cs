using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WeatherDataAnalysis.View;

namespace WeatherDataAnalysis.Model
{
    /// <summary>
    ///     Takes a list of days and returns weather data about them
    /// </summary>
    public class WeatherCalculator : ICollection<DailyStats>
    {
        #region Data members

        private const int YearPadding = 1000;
        private const int SizeFinder = 1;

        private readonly ICollection<IGrouping<int, DailyStats>> conflictingDays;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the collection of days.
        /// </summary>
        /// <value>
        ///     The collection of days.
        /// </value>
        public ICollection<DailyStats> Days { get; set; }

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

        public int Count => Days.Count;

        public bool IsReadOnly => Days.IsReadOnly;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherCalculator" /> class.
        ///     Precondition: days != null
        /// </summary>
        /// <param name="days">The days to take info from.</param>
        public WeatherCalculator(ICollection<DailyStats> days)
        {
            this.Days = days ?? throw new ArgumentNullException(nameof(days));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherCalculator" /> class.
        ///     Precondition: data != null AND days != null
        /// </summary>
        /// <param name="data">The existing data.</param>
        /// <param name="days">The new incoming data.</param>
        public WeatherCalculator(WeatherCalculator data, IEnumerable<DailyStats> days)
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
        public List<DailyStats> FindHighestTemperatureDaysOfYear(int year)
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
        public List<DailyStats> FindLowestHighTemperatureDaysOfYear(int year)
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
        public List<DailyStats> FindLowestTemperatureDaysOfYear(int year)
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
        public List<DailyStats> FindHighestLowTemperatureDaysOfYear(int year)
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
        public ICollection<MonthlyStats> GroupDaysByMonth(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).GroupBy(month => month.Date.Month)
                       .Select(group => new MonthlyStats(group))
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
        public ICollection<DailyStats> FindNextConflictingDays()
        {
            if (this.ConflictingDaysCount > 0)
            {
                return this.conflictingDays.First().ToList();
            }

            return new List<DailyStats>();
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
        /// Generates the histogram of the temps.
        /// </summary>
        /// <param name="temps">The temps.</param>
        /// <returns> the histogram</returns>
        public IDictionary<int, int> GenerateTempHistogram(List<int> temps)
        {
            var high = temps.Max();
            var low = temps.Min();

            var topOfHistogram = this.findStartingTempForHistogram(high) + this.HistogramRange;
            var bottomOfRange = this.findStartingTempForHistogram(low);

            return this.findNumbersInRange(topOfHistogram, bottomOfRange, temps);

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

        private SortedDictionary<int, int> findNumbersInRange(int topOfHistogram,
            int bottomOfRange, List<int> temps)
        {
            var groupCounts = new SortedDictionary<int, int>();
            while (bottomOfRange <= topOfHistogram)
            {
                var topOfRange = bottomOfRange + this.HistogramRange;

                var numbersInRange = temps.Count(temp => temp >= bottomOfRange && temp <= topOfRange);
                groupCounts.Add(bottomOfRange, numbersInRange);

                bottomOfRange += this.HistogramBucketSize;

            }

            return groupCounts;
        }

        /// <summary>
        /// Finds the high temperatures for year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>the high temperatures for year</returns>
        public List<int> FindHighTemperaturesForYear(int year)
        {
            var list = this.Days.Where(yearOfDay => yearOfDay.Date.Year == year).Select(day => day.HighTemperature)
                .ToList();

            return list;
        }

        /// <summary>
        /// Finds the low temperatures for year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>the low temperatures for year</returns>
        public List<int> FindLowTemperaturesForYear(int year)
        {
            return this.Days.Where(yearOfDay => yearOfDay.Date.Year == year).Select(day => day.LowTemperature)
                .ToList();
        }

        public void Add(DailyStats item)
        {
            Days.Add(item);
        }

        public void Clear()
        {
            Days.Clear();
        }

        public bool Contains(DailyStats item)
        {
            return Days.Contains(item);
        }

        public void CopyTo(DailyStats[] array, int arrayIndex)
        {
            Days.CopyTo(array, arrayIndex);
        }

        public bool Remove(DailyStats item)
        {
            return Days.Remove(item);
        }

        public IEnumerator<DailyStats> GetEnumerator()
        {
            return Days.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Days.GetEnumerator();
        }

        #endregion
    }
}