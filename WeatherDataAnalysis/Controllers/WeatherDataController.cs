using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using WeatherDataAnalysis.DataTier;
using WeatherDataAnalysis.Model;
using WeatherDataAnalysis.View;

namespace WeatherDataAnalysis.Controllers
{
    /// <summary>
    ///     Communicates between Model objects and the MainPage
    /// </summary>
    public class WeatherDataController
    {
        #region Data members

        private const ContentDialogResult Replace = ContentDialogResult.Primary;

        private WeatherCalculator weatherData;
        private DuplicateDayResult duplicateBehavior;
        private readonly WeatherDataCsvParser loader;

        private const string HighThresholdDefault = "90";
        private const string LowThresholdDefault = "32";

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the under temperature text box in the UI.
        /// </summary>
        /// <value>
        ///     The under temperature text box in the UI.
        /// </value>
        public string LowTempThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the over temperature text box in the UI.
        /// </summary>
        /// <value>
        ///     The over temperature text box in the UI.
        /// </value>
        public string HighTempThreshold { get; set; }

        /// <summary>
        ///     Gets or sets the merge or replace enum.
        /// </summary>
        /// <value>
        ///     The merge or replace enum.
        /// </value>
        public MergeOrReplaceResult MergeOrReplace { get; set; }

        /// <summary>
        ///     Gets or sets the size of the histogram bucket.
        /// </summary>
        /// <value>
        ///     The size of the histogram bucket.
        /// </value>
        public int HistogramBucketSize { get; set; }

        /// <summary>
        ///     Gets or sets the file.
        /// </summary>
        /// <value>
        ///     The file.
        /// </value>
        public StorageFile File { get; set; } 

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherDataController" /> class.
        /// </summary>
        public WeatherDataController()
        {
            this.weatherData = new WeatherCalculator(new List<DailyStats>());
            this.loader = new WeatherDataCsvParser();
            this.HighTempThreshold = HighThresholdDefault;
            this.LowTempThreshold = LowThresholdDefault;
            this.HistogramBucketSize = 10;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Populates weather data from file to be displayed.
        /// </summary>
        /// <returns>
        ///     string that will be displayed
        /// </returns>
        public async Task<string> LoadReport()
        {
            if (this.weatherData.Days.Count != 0 && this.MergeOrReplace == MergeOrReplaceResult.Merge)
            {
                this.weatherData = new WeatherCalculator(this.weatherData, await this.loader.LoadFile(this.File)) {
                    HighTemperatureThreshold = int.Parse(this.HighTempThreshold),
                    LowTemperatureThreshold = int.Parse(this.LowTempThreshold),
                    HistogramBucketSize = this.HistogramBucketSize
                };
                await this.handleDuplicateDays();
            }
            else
            {
                this.weatherData = new WeatherCalculator(await this.loader.LoadFile(this.File)) {
                    HighTemperatureThreshold = int.Parse(this.HighTempThreshold),
                    LowTemperatureThreshold = int.Parse(this.LowTempThreshold),
                    HistogramBucketSize = this.HistogramBucketSize
                };
            }

            this.duplicateBehavior = null;
            return this.refreshReport();
        }

        private async Task handleDuplicateDays()
        {
            while (this.weatherData.ConflictingDaysCount > 0)
            {
                var days = this.weatherData.FindNextConflictingDays();
                KeepOrReplace action;
                if (this.duplicateBehavior == null)
                {
                    var dialog = new DuplicateDayDialog(days.First().Date.ToShortDateString());
                    await dialog.ShowAsync();
                    if (dialog.Result.DoForAll)
                    {
                        this.duplicateBehavior = dialog.Result;
                    }

                    action = dialog.Result.KeepOrReplace;
                }
                else
                {
                    action = this.duplicateBehavior.KeepOrReplace;
                }

                this.weatherData.Merge(action);
            }
        }

        /// <summary>
        ///     Updates the output as the thresholds are changed.
        /// </summary>
        /// <returns>
        ///     Returns the output with the new threshold values
        /// </returns>
        public string UpdateThresholds()
        {
            this.resetThrehold();
            return this.refreshReport();
        }

        private void resetThrehold()
        {
            this.weatherData.HighTemperatureThreshold = int.Parse(this.HighTempThreshold);
            this.weatherData.LowTemperatureThreshold = int.Parse(this.LowTempThreshold);
        }

        /// <summary>
        ///     Clears the report.
        /// </summary>
        /// <returns>Returns an empty report</returns>
        public string ClearReport()
        {
            this.loader.LinesWithErrors = string.Empty;
            this.weatherData = new WeatherCalculator(new List<DailyStats>()) {
                HistogramBucketSize = this.HistogramBucketSize
            };
            var reportBuilder = new ReportBuilder(this.weatherData);
            return reportBuilder.Report = string.Empty;
        }

        /// <summary>
        ///     Adds the new data to this.weatherData
        ///     Precondition: data != null
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="highTemp">The high temperature.</param>
        /// <param name="lowTemp">The low temperature.</param>
        /// <param name="precipitation"> the precipitation</param>
        /// <returns>Returns output with the new data added</returns>
        public async Task<string> AddData(DateTime date, int highTemp, int lowTemp, double precipitation)
        {
            if (date == null)
            {
                throw new ArgumentNullException(nameof(date));
            }
            this.resetThrehold();
            var day = new DailyStats(date, highTemp, lowTemp, percipitation);
            var duplicate = this.weatherData.Days.FirstOrDefault(d => d.Date.Date == date.Date);
            if (duplicate == null)
            {
                this.weatherData.Days.Add(day);
            }
            else
            {
                var result = await this.generateDuplicateDialog();
                if (result == Replace)
                {
                    this.weatherData.Days.Remove(duplicate);
                    this.weatherData.Days.Add(day);
                }
            }

            return this.refreshReport();
        }

        /// <summary>
        ///     Saves the data to a csv file
        /// </summary>
        public void SaveFile()
        {
            var fileSaver = new CsvFileSaver();
            fileSaver.SaveFile(this.weatherData.Days);
        }

        /// <summary>
        ///     Updates the histogram.
        /// </summary>
        /// <returns>Returns the new report with updated histogram values</returns>
        public string UpdateHistogram()
        {
            this.weatherData.HistogramBucketSize = this.HistogramBucketSize;
            return this.refreshReport();
        }

        private async Task<ContentDialogResult> generateDuplicateDialog()
        {
            var duplicateContentDialog = new ContentDialog {
                Title = "Duplicate Day",
                Content = "Duplicate Day Exists",
                PrimaryButtonText = "Replace",
                SecondaryButtonText = "Cancel"
            };
            return await duplicateContentDialog.ShowAsync();
        }

        private string refreshReport()
        {
            var reportBuilder = new ReportBuilder(this.weatherData);
            reportBuilder.CompileReport();
            return this.loader.LinesWithErrors + reportBuilder.Report;
        }

        #endregion
    }
}