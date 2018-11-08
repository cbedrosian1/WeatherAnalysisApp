using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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
            this.summaryTextBox.Text = string.Empty;
            this.controller = new WeatherDataController();
            this.viewModel = new WeatherCalculatorDetailViewModel();
            this.DataContext = this.viewModel;
            this.radioButton10.IsChecked = true;

           
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
                   this.handleDataExists();
                }    
                this.viewModel.ReadFile(this.file);
            }
        }

        private async void handleDataExists()
        {
            var dialog = new DataExistsDialog();
            await dialog.ShowAsync();
            var mergeOrReplace = dialog.Result;
            if (mergeOrReplace == MergeOrReplaceResult.Merge)
            {
                this.viewModel.ReadNewFile(this.file);
            }
            
        }

        private void thresholdChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            this.controller.HighTempThreshold = this.overTempTextBox.Text;
            this.controller.LowTempThreshold = this.underTempTextBox.Text;

            if (string.IsNullOrEmpty(this.overTempTextBox.Text))
            {
                this.controller.HighTempThreshold = HighThresholdDefault;
            }

            if (string.IsNullOrEmpty(this.underTempTextBox.Text))
            {
                this.controller.LowTempThreshold = LowThresholdDefault;
            }

            this.summaryTextBox.Text = this.controller.UpdateThresholds();
        }

        private void histogramEventChangeHandler(object sender, RoutedEventArgs e)
        {
            if (this.radioButton5.IsChecked == true)
            {
                this.controller.HistogramBucketSize = 5;
            }

            if (this.radioButton10.IsChecked == true)
            {
                this.controller.HistogramBucketSize = 10;
            }

            if (this.radioButton20.IsChecked == true)
            {
                this.controller.HistogramBucketSize = 20;
            }

            this.summaryTextBox.Text = this.controller.UpdateHistogram();
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            this.controller.SaveFile();
        }

        #endregion
    }
}