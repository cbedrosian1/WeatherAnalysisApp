using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Gets or sets the conflicting days.
        /// </summary>
        /// <value>
        /// The conflicting days.
        /// </value>
        public ICollection<IGrouping<int, DailyStats>> ConflictingDays { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the collection of days.
        /// </summary>
        /// <value>
        ///     The collection of days.
        /// </value>
        public IList<DailyStats> Days { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DailyStats"/> with the specified i.
        /// </summary>
        /// <value>
        /// The <see cref="DailyStats"/>.
        /// </value>
        /// <param name="i">The index.</param>
        /// <returns></returns>
        public DailyStats this[int i]
        {
            get => this.Days[i];
            set => this.Days[i] = value;
        }

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
        public int ConflictingDaysCount => this.ConflictingDays.Count;

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

        /// <summary>
        /// Gets the count of days 
        /// </summary>
        /// <value>
        /// the count of days
        /// </value>
        public int Count => this.Days.Count;

        /// <summary>
        /// gets whether true or false depending on if days is read only
        /// </summary>
        /// <value> true or false depending on days is read only</value>
        public bool IsReadOnly => this.Days.IsReadOnly;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherCalculator" /> class.
        ///     Precondition: days != null
        /// </summary>
        /// <param name="days">The days to take info from.</param>
        public WeatherCalculator(IList<DailyStats> days)
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
            this.ConflictingDays = groupedDays.Where(group => group.Count() > 1).ToList();
            this.Days = this.Days.Union(groupedDays.Where(group => group.Count() == 1).Select(group => group.First()))
                            .ToList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the day with date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public DailyStats FindDayWithDate(DateTime date)
        {
            try
            {
                return this.Days.First(day => day.Date.ToShortDateString() == date.ToShortDateString());
            }
            catch (InvalidOperationException)
            {
                return null;
            }

        }

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
        ///     Gets the average temperature of the provided temperatures.
        /// </summary>
        /// <param name="temps">The temperatures.</param>
        /// <returns>
        ///     Returns the average temperature of the list
        /// </returns>
        public double FindAverageTemperatureOfYear(List<int> temps)
        { 
            return temps.Average();
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
        /// Gets the day that has the highest precipitation
        /// </summary>
        /// <param name="year"> the year that the day occurs in</param>
        /// <returns> the day with the highest precipitation</returns>
        public List<DailyStats> FindDayHighestPrecipitationOccuredOn(int year)
        {
            return this.Days.Where(day => day.Date.Year == year).OrderBy(day => day.Date).GroupBy(day => day.Precipitation)
                       .OrderByDescending(precipitation => precipitation.Key).First().ToList();

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
        public ICollection<DateTime> FindYears()
        {
            var years = new List<DateTime>();
            var yearGroups = this.Days.GroupBy(day => day.Date.Year);

            foreach (var currentYear in yearGroups)
            {
                years.Add(new DateTime(currentYear.First().Date.Year, 1, 1));
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
                return this.ConflictingDays.First().ToList();
            }

            return new List<DailyStats>();
        }

        /// <summary>
        /// Replaces days in list with new conflicting days
        /// </summary>
        /// <param name="conflictingDays">The conflicting days.</param>
        public void ReplaceOriginalDaysWithDuplicateDays(IGrouping<int, DailyStats> conflictingDays)
        {
       
                foreach (var currentDay in conflictingDays)
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

            this.ConflictingDays.Remove(conflictingDays);
        }

        /// <summary>
        /// Keeps the original days while removing conflicting days
        /// </summary>
        /// <param name="conflictingDays">The conflicting days.</param>
        public void KeepOriginalDays(IGrouping<int, DailyStats> conflictingDays)
        {
            this.ConflictingDays.Remove(conflictingDays);
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
            return this.Days.Where(yearOfDay => yearOfDay.Date.Year == year).Select(day => day.HighTemperature)
                       .ToList();
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

        /// <summary>
        /// Adds the specified day.
        /// </summary>
        /// <param name="day">The day.</param>
        public void Add(DailyStats day)
        {
            this.Days.Add(day);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Clear()
        {
            this.Days.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(DailyStats item)
        {
            return this.Days.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(DailyStats[] array, int arrayIndex)
        {
            this.Days.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if item was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if item is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        public bool Remove(DailyStats item)
        {
            return this.Days.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DailyStats> GetEnumerator()
        {
            return this.Days.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Days.GetEnumerator();
        }

        #endregion
    }
}