using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WeatherDataAnalysis.Controllers;
using WeatherDataAnalysis.View;

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
        public const int ApplicationHeight = 450;

        /// <summary>
        ///     The application width
        /// </summary>
        public const int ApplicationWidth = 1150;

        private StorageFile file;

        private readonly WeatherDataController controller;

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
            this.file = await openPicker.PickSingleFileAsync();
            if (this.file != null)
            {
                this.controller.File = this.file;

                if (!string.IsNullOrEmpty(this.summaryTextBox.Text))
                {
                    this.controller.MergeOrReplace = await this.handleDataExists();
                }

                this.summaryTextBox.Text = await this.controller.LoadReport();
            }
        }

        private async Task<MergeOrReplaceResult> handleDataExists()
        {
            var dialog = new DataExistsDialog();
            await dialog.ShowAsync();
            return dialog.Result;
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

        private void clearData_Click(object sender, RoutedEventArgs e)
        {
            this.summaryTextBox.Text = this.controller.ClearReport();
        }

        private async void addDay_Click(object sender, RoutedEventArgs e)
        {
            var date = DateTime.Parse(this.dateTextBox.Text);
            var highTemp = int.Parse(this.highTempTextBox.Text);
            var lowTemp = int.Parse(this.lowTempTextBox.Text);

            if (highTemp > lowTemp)
            {
                this.summaryTextBox.Text = await this.controller.AddData(date, highTemp, lowTemp);
                this.tempCheckerTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tempCheckerTextBlock.Visibility = Visibility.Visible;
            }
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