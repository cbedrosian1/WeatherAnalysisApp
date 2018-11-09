using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using WeatherDataAnalysis.View;
using WeatherDataAnalysis.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherDataAnalysis
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        /// <summary>
        ///     The application height
        /// </summary>
        public const int ApplicationHeight = 425;

        /// <summary>
        ///     The application width
        /// </summary>
        public const int ApplicationWidth = 1100;

        /// <summary>
        ///     The view model
        /// </summary>
        public readonly WeatherCalculatorDetailViewModel ViewModel;

        private DuplicateDayResult duplicateBehavior;
        private StorageFile file;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size {Width = ApplicationWidth, Height = ApplicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(ApplicationWidth, ApplicationHeight));
            this.ViewModel = new WeatherCalculatorDetailViewModel();
            DataContext = this.ViewModel;
        }

        #endregion

        #region Methods

        private async void loadFile_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            openPicker.FileTypeFilter.Add(".csv");
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".xml");
            this.file = await openPicker.PickSingleFileAsync();
            if (this.file != null)
            {
                if (this.ViewModel.Days.Count > 0)
                {
                    await this.handleDataExists();
                }
                else
                {
                    this.ViewModel.ReadFile(this.file);
                }

                this.duplicateBehavior = null;
            }
        }

        private async Task handleDataExists()
        {
            var dialog = new DataExistsDialog();
            await dialog.ShowAsync();
            var mergeOrReplace = dialog.Result;
            if (mergeOrReplace == MergeOrReplaceResult.Merge)
            {
                await this.ViewModel.ReadNewFile(this.file);
                await this.handleDuplicateDays();
                this.ViewModel.UpdateDays();
            }
            else
            {
                this.ViewModel.ReadFile(this.file);
            }
        }

        private async Task handleDuplicateDays()
        {
            while (this.ViewModel.FindDuplicateDays().Count > 0)
            {
                var days = this.ViewModel.FindNextConflictingDays();
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

                if (action == KeepOrReplace.Replace)
                {
                    this.ViewModel.Merge(true);
                }
                else if (action == KeepOrReplace.Keep)
                {
                    this.ViewModel.Merge(false);
                }
            }
        }

        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("CSV", new List<string> {".csv"});
            savePicker.FileTypeChoices.Add("XML", new List<string> {".xml"});
            savePicker.SuggestedFileName = "New Document";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                this.ViewModel.SaveFile(file);
            }
        }

        private void summaryButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SummaryPage), this);
        }

        private void allYearsButton_Click(object sender, RoutedEventArgs e)
        {
            this.yearsDropDownBox.SelectedItem = null;
            this.ViewModel.SelectedYear = 1;
        }

        #endregion
    }
}