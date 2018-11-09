using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    ///     The view model for the weather calculator
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class WeatherCalculatorDetailViewModel : INotifyPropertyChanged
    {
        #region Data members

        private DateTime date;

        private int highTemperature;

        private int lowTemperature;

        private double precipitation;

        private DailyStats selectedDay;

        private ObservableCollection<DailyStats> days;

        private readonly WeatherDataParser parser;

        private int selectedYear;

        private ObservableCollection<DateTime> years;

        private ObservableCollection<DailyStats> selectedDays;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the weather calculator.
        /// </summary>
        /// <value>
        ///     The weather calculator.
        /// </value>
        public WeatherCalculator WeatherCalculator { get; private set; }

        /// <summary>
        ///     Gets or sets the selected days.
        /// </summary>
        /// <value>
        ///     The selected days.
        /// </value>
        public ObservableCollection<DailyStats> SelectedDays
        {
            get => this.selectedDays;
            set
            {
                this.selectedDays = value;
                this.OnPropertyChanged();
                this.ClearDataCommand?.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the selected year.
        /// </summary>
        /// <value>
        ///     The selected year.
        /// </value>
        public int SelectedYear
        {
            get => this.selectedYear;
            set
            {
                this.selectedYear = value;
                this.OnPropertyChanged();
                if (this.SelectedYear == 1)
                {
                    this.SelectedDays = this.Days;
                }
                else
                {
                    this.SelectedDays = this.Days.Where(day => day.Date.Year == this.SelectedYear)
                                            .ToObservableCollection();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the years.
        /// </summary>
        /// <value>
        ///     The years.
        /// </value>
        public ObservableCollection<DateTime> Years
        {
            get => this.years;
            set
            {
                this.years = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the remove command.
        /// </summary>
        /// <value>
        ///     The remove command.
        /// </value>
        public RelayCommand RemoveCommand { get; set; }

        /// <summary>
        ///     Gets or sets the add command.
        /// </summary>
        /// <value>
        ///     The add command.
        /// </value>
        public RelayCommand AddCommand { get; set; }

        /// <summary>
        ///     Gets or sets the edit command.
        /// </summary>
        /// <value>
        ///     The edit command.
        /// </value>
        public RelayCommand EditCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear data command.
        /// </summary>
        /// <value>
        ///     The clear data command.
        /// </value>
        public RelayCommand ClearDataCommand { get; set; }

        /// <summary>
        ///     Gets or sets the date.
        /// </summary>
        /// <value>
        ///     The date.
        /// </value>
        public DateTime Date
        {
            get => this.date;
            set
            {
                this.date = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
                this.EditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the high temperature.
        /// </summary>
        /// <value>
        ///     The high temperature.
        /// </value>
        public int HighTemperature
        {
            get => this.highTemperature;
            set
            {
                this.highTemperature = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
                this.EditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the low temperature.
        /// </summary>
        /// <value>
        ///     The low temperature.
        /// </value>
        public int LowTemperature
        {
            get => this.lowTemperature;
            set
            {
                this.lowTemperature = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
                this.EditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the precipitation.
        /// </summary>
        /// <value>
        ///     The precipitation.
        /// </value>
        public double Precipitation
        {
            get => this.precipitation;
            set
            {
                this.precipitation = value;
                this.OnPropertyChanged();
                this.AddCommand.OnCanExecuteChanged();
                this.EditCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the selected day.
        /// </summary>
        /// <value>
        ///     The selected day.
        /// </value>
        public DailyStats SelectedDay
        {
            get => this.selectedDay;
            set
            {
                this.selectedDay = value;
                this.OnPropertyChanged();
                this.RemoveCommand.OnCanExecuteChanged();
                this.EditCommand.OnCanExecuteChanged();
                this.Date = this.selectedDay.Date;
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
                this.ClearDataCommand?.OnCanExecuteChanged();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="WeatherCalculatorDetailViewModel" /> class.
        /// </summary>
        public WeatherCalculatorDetailViewModel()
        {
            this.date = DateTime.Now;
            this.parser = new WeatherDataParser();
            this.WeatherCalculator = new WeatherCalculator(new List<DailyStats>());
            this.Days = this.WeatherCalculator.Days.ToObservableCollection();
            this.selectedDays = this.Days.Where(day => day.Date.Year == this.SelectedYear).ToObservableCollection();
            this.initializeCommands();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns> the event</returns>
        public event PropertyChangedEventHandler PropertyChanged;

        private void initializeCommands()
        {
            this.RemoveCommand = new RelayCommand(this.deleteDay, this.canDeleteDay);
            this.AddCommand = new RelayCommand(this.addDay, this.canAddDay);
            this.EditCommand = new RelayCommand(this.editDay, this.canEditDay);
            this.ClearDataCommand = new RelayCommand(this.clearData, this.canClearData);
        }

        private bool canClearData(object obj)
        {
            return this.Days.Count > 0;
        }

        private void clearData(object obj)
        {
            this.WeatherCalculator.Days.Clear();
            this.Years.Clear();
            this.UpdateDays();
        }

        private bool canEditDay(object obj)
        {
            return this.HighTemperature > this.LowTemperature && this.SelectedDay != null &&
                   this.SelectedDay.Date == this.Date;
        }

        private void editDay(object obj)
        {
            var index = this.WeatherCalculator.Days.IndexOf(this.SelectedDay);
            this.WeatherCalculator.Days[index].HighTemperature = this.HighTemperature;
            this.WeatherCalculator.Days[index].LowTemperature = this.LowTemperature;
            this.WeatherCalculator.Days[index].Precipitation = this.Precipitation;

            this.Days = this.WeatherCalculator.Days.ToObservableCollection();
        }

        private bool canAddDay(object obj)
        {
            return this.HighTemperature > this.LowTemperature &&
                   this.WeatherCalculator.FindDayWithDate(this.Date) == null;
        }

        private void addDay(object obj)
        {
            var dayToAdd = new DailyStats(this.Date, this.HighTemperature, this.LowTemperature,
                this.Precipitation);
            this.WeatherCalculator.Add(dayToAdd);
            this.WeatherCalculator.Days = this.WeatherCalculator.Days.OrderBy(day => day.Date).ToList();
            this.UpdateDays();
        }

        private bool canDeleteDay(object obj)
        {
            return this.SelectedDay != null;
        }

        private void deleteDay(object obj)
        {
            this.WeatherCalculator.Remove(this.SelectedDay);
            this.UpdateDays();
        }

        /// <summary>
        ///     Finds the lines with errors.
        /// </summary>
        /// <returns></returns>
        public string FindLinesWithErrors()
        {
            return this.parser.LinesWithErrors;
        }

        /// <summary>
        ///     Reads the file.
        /// </summary>
        /// <param name="file">The file to be read.</param>
        public async Task ReadFile(StorageFile file)
        {
            this.WeatherCalculator = new WeatherCalculator(await this.parser.LoadFile(file));
            this.UpdateDays();
        }

        /// <summary>
        ///     Reads a file when data is already present
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task ReadNewFile(StorageFile file)
        {
            this.WeatherCalculator = new WeatherCalculator(this.WeatherCalculator, await this.parser.LoadFile(file));
            this.UpdateDays();
        }

        /// <summary>
        ///     Saves the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void SaveFile(StorageFile file)
        {
            var fileSaver = new WeatherDataFileSaver();
            fileSaver.SaveFile(this.WeatherCalculator.Days, file);
        }

        /// <summary>
        ///     Finds the duplicate days.
        /// </summary>
        /// <returns>Collection of groupings of duplicate Days</returns>
        public ICollection<IGrouping<int, DailyStats>> FindDuplicateDays()
        {
            return this.WeatherCalculator.ConflictingDays;
        }

        /// <summary>
        ///     Updates the days.
        /// </summary>
        public void UpdateDays()
        {
            this.Years = this.WeatherCalculator.FindYears().ToList().ToObservableCollection();
            this.WeatherCalculator.Days = this.WeatherCalculator.Days.OrderBy(day => day.Date).ToList();
            this.Days = this.WeatherCalculator.Days.ToObservableCollection();
            this.SelectedDays = this.Days;
        }

        /// <summary>
        ///     Finds the next conflicting days.
        /// </summary>
        /// <returns>Returns collection of conflicting days</returns>
        public ICollection<DailyStats> FindNextConflictingDays()
        {
            return this.WeatherCalculator.FindNextConflictingDays();
        }

        /// <summary>
        ///     Calls the weather calculator ReplaceOriginalDaysWithDuplicateDays method
        /// </summary>
        public void ReplaceOriginalDaysWithDuplicateDays()
        {
            this.WeatherCalculator.ReplaceOriginalDaysWithDuplicateDays(this.WeatherCalculator.ConflictingDays.First());
        }

        /// <summary>
        ///     Calls the weather calculator KeepOriginalDays method
        /// </summary>
        public void KeepOriginalDays()
        {
            this.WeatherCalculator.KeepOriginalDays(this.WeatherCalculator.ConflictingDays.First());
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