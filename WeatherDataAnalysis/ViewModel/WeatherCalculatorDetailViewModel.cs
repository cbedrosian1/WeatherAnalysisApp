using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.ViewModel
{
    /// <summary>
    /// The view model for the weather calculator 
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WeatherCalculatorDetailViewModel : INotifyPropertyChanged
    {
        private WeatherCalculator weatherCalculator;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns> the event</returns>
        public event PropertyChangedEventHandler PropertyChanged;

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
            this.days = new ObservableCollection<DailyStats>();
            this.weatherCalculator = new WeatherCalculator(this.days);
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
