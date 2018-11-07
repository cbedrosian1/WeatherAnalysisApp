using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using WeatherDataAnalysis.DataTier;
using WeatherDataAnalysis.Extension;
using WeatherDataAnalysis.Model;
using WeatherDataAnalysis.Utility;

namespace WeatherDataAnalysis.ViewModel
{
    /// <summary>
    /// The view model for the weather calculator 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WeatherCalculatorDetailViewModel : INotifyPropertyChanged
    {
        private WeatherCalculator weatherCalculator;


        public RelayCommand RemoveCommand { get; set; }
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns> the event</returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private DailyStats selectedDay;

        public DailyStats SelectedDay
        {
            get => this.selectedDay; 
            set
            {
                this.selectedDay = value;
                this.OnPropertyChanged();
                this.RemoveCommand.OnCanExecuteChanged();
            }
        }

        private ObservableCollection<DailyStats> days;

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>
        /// The days.
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

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherCalculatorDetailViewModel"/> class.
        /// </summary>
        public WeatherCalculatorDetailViewModel()
        {
            this.weatherCalculator = new WeatherCalculator(new List<DailyStats>());
            this.Days = this.weatherCalculator.Days.ToObservableCollection();
            this.RemoveCommand = new RelayCommand(this.DeleteDay, this.CanDeleteDay);
        }

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
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
