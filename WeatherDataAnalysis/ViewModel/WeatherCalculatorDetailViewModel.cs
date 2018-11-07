using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Microsoft.Toolkit.Extensions;
using WeatherDataAnalysis.DataTier;
using WeatherDataAnalysis.Extension;
using WeatherDataAnalysis.Model;
using WeatherDataAnalysis.Utility;

namespace WeatherDataAnalysis.ViewModel
{
    /// <summary>
    ///     The view model for the weather calculator
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WeatherCalculatorDetailViewModel : INotifyPropertyChanged
    {
        #region Data members

        private readonly WeatherCalculator weatherCalculator;

        private DateTimeOffset date;

        private int highTemperature;

        private int lowTemperature;

        private double precipitation;

        private DailyStats selectedDay;

        private ObservableCollection<DailyStats> days;

        #endregion

        #region Properties

        public RelayCommand RemoveCommand { get; set; }

        public RelayCommand AddCommand { get; set; }

        public RelayCommand EditCommand { get; set; }

        public DateTimeOffset Date
        {
            get => this.date;
            set
            {
                this.date = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
            }
        }

        public int HighTemperature
        {
            get => this.highTemperature;
            set
            {
                this.highTemperature = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
            }
        }

        public int LowTemperature
        {
            get => this.lowTemperature;
            set
            {
                this.lowTemperature = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
            }
        }

        public double Precipitation
        {
            get => this.precipitation;
            set
            {
                this.precipitation = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
            }
        }

        public DailyStats SelectedDay
        {
            get => this.selectedDay;
            set
            {
                this.selectedDay = value;
                this.OnPropertyChanged();
                this.RemoveCommand.OnCanExecuteChanged();
                this.Date = this.selectedDay.DateTimeOffset;
                this.HighTemperature = this.selectedDay.HighTemperature;
                this.LowTemperature = this.selectedDay.LowTemperature;
                this.Precipitation = this.selectedDay.Precipitation;
                
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
        public WeatherCalculatorDetailViewModel()
        {
            this.date = DateTimeOffset.Now;
            this.highTemperature = 0;
            this.lowTemperature = 0;
            this.precipitation = 0.0;
            this.weatherCalculator = new WeatherCalculator(new List<DailyStats>());
            this.Days = this.weatherCalculator.Days.ToObservableCollection();
            this.RemoveCommand = new RelayCommand(this.DeleteDay, this.CanDeleteDay);
            this.AddCommand = new RelayCommand(this.AddDay, this.CanAddDay);
            this.EditCommand = new RelayCommand(this.EditDay, this.CanEditDay);
        }

        private bool CanEditDay(object obj)
        {
            throw new NotImplementedException();
        }

        private void EditDay(object obj)
        {
            //TODO
        }

        private bool CanAddDay(object obj)
        {
            return true;
        }

        private void AddDay(object obj)
        {
            var dayToAdd = new DailyStats(this.Date.DateTime, this.HighTemperature, this.LowTemperature, this.Precipitation);
            this.weatherCalculator.Add(dayToAdd);
            this.Days = this.weatherCalculator.Days.ToObservableCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns> the event</returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private bool CanDeleteDay(object obj)
        {
            return this.SelectedDay != null;
        }

        private void DeleteDay(object obj)
        {
            this.weatherCalculator.Remove(this.SelectedDay);
            this.Days = this.weatherCalculator.Days.ToObservableCollection();
        }

        public async void ReadFileAsync(StorageFile file)
        {
            var parser = new WeatherDataParser();
            var parsedDays = await parser.LoadFile(file);
            this.weatherCalculator.Days = parsedDays;
            this.Days = parsedDays.ToObservableCollection();
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