using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using WeatherDataAnalysis.Controllers;
using WeatherDataAnalysis.View;
using WeatherDataAnalysis.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WeatherDataAnalysis
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    ///
    /// 
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

        private StorageFile file;

        private readonly WeatherDataController controller;

        private readonly WeatherCalculatorDetailViewModel viewModel;

        private const string HighThresholdDefault = "90";
        private const string LowThresholdDefault = "32";

        private DuplicateDayResult duplicateBehavior;

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
            this.viewModel = new WeatherCalculatorDetailViewModel();
            this.DataContext = this.viewModel; 

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
                if (this.viewModel.Days.Count > 0)
                {
                    await this.handleDataExists();
                }
                else
                {
                    this.viewModel.ReadFile(this.file);
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
                await this.viewModel.ReadNewFile(this.file);
                await this.handleDuplicateDays();
                this.viewModel.UpdateDays();
            }
            else
            {
                this.viewModel.ReadFile(this.file);
            }
        }

        private async Task handleDuplicateDays()
        {
            while (this.viewModel.FindDuplicateDays().Count > 0)
            {
                var days = this.viewModel.FindNextConflictingDays();
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
                    this.viewModel.Merge(true);
                }
                else if(action == KeepOrReplace.Keep)
                {

                    this.viewModel.Merge(false);
                } 
            }
            
        }

        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("CSV", new List<string> { ".csv" });
            savePicker.FileTypeChoices.Add("XML", new List<string> { ".xml" });
            savePicker.SuggestedFileName = "New Document";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                this.viewModel.SaveFile(file);
            }
        }

        #endregion


        private void summaryButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SummaryPage), this.viewModel);
        }

        private async Task open()
        {
            var dialog = new SummaryDialog();
            await dialog.ShowAsync();
        }

  
    }
}