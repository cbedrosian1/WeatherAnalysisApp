using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.View
{
    /// <summary>
    ///     Formats a report to be displayed in text box
    /// </summary>
    public class ReportBuilder
    {
        #region Data members

        private const int FirstIndex = 0;
        private const int MonthCount = 12;
        private const string High = "High";
        private const string Low = "Low";
        private readonly WeatherCalculator data;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the report to be displayed to text box.
        /// </summary>
        /// <value>
        ///     The report to be displayed to text box.
        /// </value>
        public string Report { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportBuilder" /> class.
        /// </summary>
        /// <param name="data">WeatherData being passed to the report.</param>
        /// <exception cref="ArgumentNullException">collection</exception>
        public ReportBuilder(WeatherCalculator data)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Compiles the report to be displayed.
        /// </summary>
        public void CompileReport()
        {
            var years = this.data.FindYears();
            foreach (var currentYear in years)
            {
                var highTemps = this.data.FindHighTemperaturesForYear(currentYear);
                var lowTemps = this.data.FindLowTemperaturesForYear(currentYear);

                this.Report += $"Year: {currentYear}" + Environment.NewLine;
                this.addHighestTemperatureDaysOfYearToReport(currentYear);
                this.addLowestTemperatureDaysOfYearToReport(currentYear);
                this.addLowestHighTemperatureDaysOfYearToReport(currentYear);
                this.addHighestLowTemperatureDaysOfYearToReport(currentYear);
                this.addAverageTemperatureOfYearToReport(highTemps, High);
                this.addAverageTemperatureOfYearToReport(lowTemps, Low);
                this.addNumberOfDaysOverTemperatureToReport(currentYear);
                this.addNumberOfDaysUnderTemperatureToReport(currentYear);
                this.addTempHistogramToReport(highTemps, High);
                this.addTempHistogramToReport(lowTemps, Low);
                this.addInfoForMonthsToReport(currentYear);
                this.Report += Environment.NewLine;
            }
        }

        private void addHighestTemperatureDaysOfYearToReport(int year)
        {
            var days = this.data.FindHighestTemperatureDaysOfYear(year);

            this.Report +=
                $"The highest temperature of the year was {days[FirstIndex].HighTemperature} and it occured on date(s): {string.Join(", ", days.Select(d => d.Date.ToShortDateString()))}" +
                Environment.NewLine;
        }

        private void addLowestTemperatureDaysOfYearToReport(int year)
        {
            var days = this.data.FindLowestTemperatureDaysOfYear(year);
            this.Report +=
                $"The lowest temperature of the year was {days[FirstIndex].LowTemperature} and it occured on date(s): {string.Join(", ", days.Select(d => d.Date.ToShortDateString()))}" +
                Environment.NewLine;
        }

        private void addLowestHighTemperatureDaysOfYearToReport(int year)
        {
            var days = this.data.FindLowestHighTemperatureDaysOfYear(year);
            this.Report +=
                $"The lowest high temperature was {days[FirstIndex].HighTemperature} and it occured on date(s): {string.Join(", ", days.Select(d => d.Date.ToShortDateString()))}" +
                Environment.NewLine;
        }

        private void addHighestLowTemperatureDaysOfYearToReport(int year)
        {
            var days = this.data.FindHighestLowTemperatureDaysOfYear(year);
            this.Report +=
                $"The highest low temperature was {days[FirstIndex].LowTemperature} and it occured on date(s): {string.Join(", ", days.Select(d => d.Date.ToShortDateString()))}" +
                Environment.NewLine;
        }

        private void addAverageTemperatureOfYearToReport(List<int> temps, string highOrLow)
        {
            var days = this.data.FindAverageTemperatureOfYear(temps);
            this.Report += $"The average {highOrLow.ToLower()} temperature of the year was: {days:0.00}" + Environment.NewLine;
        }

        private void addNumberOfDaysOverTemperatureToReport(int year)
        {
            var temperatureThreshold = this.data.HighTemperatureThreshold;
            var days = this.data.FindNumberOfDaysOverTemperature(year);
            this.Report +=
                $"The number of days with a temperature {temperatureThreshold} degrees or higher is: {days}" +
                Environment.NewLine;
        }

        private void addNumberOfDaysUnderTemperatureToReport(int year)
        {
            var temperatureThreshold = this.data.LowTemperatureThreshold;
            var days = this.data.FindNumberOfDaysUnderTemperature(year);
            this.Report += $"The number of days with a temperature {temperatureThreshold} degrees or lower is: {days}" +
                           Environment.NewLine;
        }

        private void addInfoForMonthsToReport(int year)
        {
            var textToBeReported = string.Empty;
            var months = this.data.GroupDaysByMonth(year);

            for (var currentMonth = 1; currentMonth <= MonthCount; currentMonth++)
            {
                var month = months.FirstOrDefault(m => m.MonthNumber == currentMonth);
                var daysInMonth = 0;
                if (month != null)
                {
                    daysInMonth = month.DaysInMonth;
                }

                textToBeReported +=
                    Environment.NewLine + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentMonth) +
                    $" {year} ({daysInMonth} day(s) of data) " + Environment.NewLine;
                if (month != null)
                {
                    var averageHighTemp = month.AverageHighTemperature;
                    var averageLowTemp = month.AverageLowTemperature;
                    textToBeReported +=
                        $"Highest temp: {month.HighTempDays[FirstIndex].HighTemperature} occured on the {this.addSuffixToDays(month.HighTempDays)}" +
                        Environment.NewLine +
                        $"Lowest temp: {month.LowTempDays[FirstIndex].LowTemperature} occured on the {this.addSuffixToDays(month.LowTempDays)}" +
                        Environment.NewLine +
                        $"Average High: {averageHighTemp:0.00}" + Environment.NewLine +
                        $"Average Low: {averageLowTemp:0.00}" + Environment.NewLine;
                }
            }

            this.Report += textToBeReported;
        }

        private string addSuffixToDays(IEnumerable<DailyStats> daysInMonth)
        {
            var days = new List<string>();

            foreach (var day in daysInMonth)
            {
                var daySuffix = day.Date.Day + this.getSuffix(day);
                days.Add(daySuffix);
            }

            return string.Join(", ", days);
        }

        private string getSuffix(DailyStats day)
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

        private void addTempHistogramToReport(List<int> temps, string highOrLow)
        {
            this.Report += highOrLow + " temperature Histogram: " + Environment.NewLine;
            var temperatureHistogram = this.data.GenerateTempHistogram(temps);
            foreach (var currentKey in temperatureHistogram.Keys)
            {
                this.Report +=
                    $"{currentKey} - {currentKey + this.data.HistogramRange}: {temperatureHistogram[currentKey]}" +
                    Environment.NewLine;
            }
        }

        #endregion
    }
}