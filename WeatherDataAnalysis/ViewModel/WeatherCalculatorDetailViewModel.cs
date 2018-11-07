using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using WeatherDataAnalysis.DataTier;
using WeatherDataAnalysis.Extension;
using WeatherDataAnalysis.Model;

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

        private DailyStats selectedDay;

        private ObservableCollection<DailyStats> days;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected day.
        /// </summary>
        /// <value>
        /// The selected day.
        /// </value>
        public DailyStats SelectedDay
        {
            get => this.selectedDay;
            set
            {
                this.selectedDay = value;
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
        public WeatherCalculatorDetailViewModel()
        {
            this.weatherCalculator = new WeatherCalculator(new List<DailyStats>());

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
        ///     Reads the file asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task ReadFileAsync(StorageFile file)
        {
            var parser = new WeatherDataParser();
            var test = await parser.LoadFile(file);
            foreach (var current in test)
            {
                this.weatherCalculator.Add(current);
            }

            this.Days = this.weatherCalculator.Days.ToObservableCollection();
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