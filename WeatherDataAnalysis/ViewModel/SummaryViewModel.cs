using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WeatherDataAnalysis.Model;
using WeatherDataAnalysis.Utility;
using WeatherDataAnalysis.View;

namespace WeatherDataAnalysis.ViewModel
{
    /// <summary>
    /// View model for the summary window
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    class SummaryViewModel : INotifyPropertyChanged
    {
        private WeatherCalculator weatherCalculator;
        private int bucketSize;
        private string report;
        private int lowTempThreshold;
        private int highTempThreshold;
        private ObservableCollection<DailyStats> days;



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
                this.SummaryCommand?.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the bucket.
        /// </summary>
        /// <value>
        /// The size of the bucket.
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
        ///     Gets or sets the high temperature threshold.
        /// </summary>
        /// <value>
        ///     The high temperature threshold.
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

        /// <summary>
        ///     Gets or sets the low temperature threshold.
        /// </summary>
        /// <value>
        ///     The low temperature threshold.
        /// </value>
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
        ///     Gets or sets the report.
        /// </summary>
        /// <value>
        ///     The report.
        /// </value>
        public string Report
        {
            get => this.report;

            set
            {
                this.report = value;
                this.OnPropertyChanged();
                this.SummaryCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the summary command.
        /// </summary>
        /// <value>
        ///     The summary command.
        /// </value>
        public RelayCommand SummaryCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public SummaryViewModel()
        {
            this.SummaryCommand = new RelayCommand(this.createSummary, this.canCreateSummary);
        }

        private bool canCreateSummary(object obj)
        {
            return this.Days.Count > 0;
        }

        private void createSummary(object obj)
        {
            this.refreshThreshold();
            this.buildReport();
        }

        private void refreshThreshold()
        {
            this.weatherCalculator.HighTemperatureThreshold = 90;
            this.weatherCalculator.LowTemperatureThreshold = 32;
            this.weatherCalculator.HistogramBucketSize = 10;
            this.HighTempThreshold = 90;
            this.LowTempThreshold = 32;
        }

        private void buildReport()
        {
            var reportBuilder = new ReportBuilder(this.weatherCalculator);
            reportBuilder.CompileReport();
            this.Report = reportBuilder.Report;
        }

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
