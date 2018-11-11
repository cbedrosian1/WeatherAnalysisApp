using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherDataAnalysis.Extension;
using WeatherDataAnalysis.Model;
using WeatherDataAnalysis.View;

namespace WeatherDataAnalysis.ViewModel
{
    /// <summary>
    ///     View model for the summary window
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    internal class SummaryViewModel : INotifyPropertyChanged
    {
        #region Data members

        private const int DefaultHighTemp = 90;

        private const int DefaultLowTemp = 32;

        private const int DefaultBucketSize = 10;

        private WeatherCalculator weatherCalculator;

        private ObservableCollection<DailyStats> days;

        private string report;

        private int highTempThreshold;

        private int lowTempThreshold;

        private int bucketSize;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the weather calculator.
        /// </summary>
        /// <value>
        ///     The weather calculator.
        /// </value>
        public WeatherCalculator WeatherCalculator
        {
            get => this.weatherCalculator;
            set
            {
                this.weatherCalculator = value;
                this.OnPropertyChanged();
            }
        }

        public ReportBuilder ReportBuilder { get; private set; }

        /// <summary>
        ///     Gets or sets the size of the bucket.
        /// </summary>
        /// <value>
        ///     The size of the bucket.
        /// </value>
        public int BucketSize
        {
            get => this.bucketSize;
            set
            {
                this.bucketSize = value;
                this.OnPropertyChanged();

                if (this.weatherCalculator != null)
                {
                    this.weatherCalculator.HistogramBucketSize = this.BucketSize;
                    this.buildReport();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the high temporary threshold.
        /// </summary>
        /// <value>
        ///     The high temporary threshold.
        /// </value>
        public int HighTempThreshold
        {
            get => this.highTempThreshold;
            set
            {
                this.highTempThreshold = value;
                this.OnPropertyChanged();
                if (this.weatherCalculator != null)
                {
                    this.weatherCalculator.HighTemperatureThreshold = this.HighTempThreshold;
                    this.buildReport();
                }
            }
        }

        // <summary>
        //     Gets or sets the low temperature threshold.
        // </summary>
        // <value>
        //     The low temperature threshold.
        // </value>
        public int LowTempThreshold
        {
            get => this.lowTempThreshold;
            set
            {
                this.lowTempThreshold = value;
                this.OnPropertyChanged();
                if (this.weatherCalculator != null)
                {
                    this.weatherCalculator.LowTemperatureThreshold = this.LowTempThreshold;
                    this.buildReport();
                }
            }
        }

        /// <summary>
        ///     gets or sets the report.
        /// </summary>
        /// <value>
        ///     the report.
        /// </value>
        public string Report
        {
            get => this.report;

            set
            {
                this.report = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the days.
        /// </summary>
        /// <value>
        ///     The days.
        /// </value>
        public ObservableCollection<DailyStats> Days
        {
            get => this.days;
            set
            {
                this.days = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherCalculatorDetailViewModel" /> class.
        /// </summary>
        public SummaryViewModel()
        {
            this.weatherCalculator = new WeatherCalculator(new List<DailyStats>());
            this.ReportBuilder = new ReportBuilder(this.weatherCalculator);
            this.Days = this.weatherCalculator.Days.ToObservableCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns> the event</returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Creates the summary.
        /// </summary>
        public void CreateSummary()

        {
            this.refreshThreshold();
            this.buildReport();
        }

        private void refreshThreshold()
        {
            this.weatherCalculator.HighTemperatureThreshold = DefaultHighTemp;
            this.weatherCalculator.LowTemperatureThreshold = DefaultLowTemp;
            this.weatherCalculator.HistogramBucketSize = DefaultBucketSize;
            this.HighTempThreshold = DefaultHighTemp;
            this.LowTempThreshold = DefaultLowTemp;
        }

        private void buildReport()
        {
            this.ReportBuilder = new ReportBuilder(this.weatherCalculator);
            this.ReportBuilder.CompileReport();
            this.Report = this.ReportBuilder.Report;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}