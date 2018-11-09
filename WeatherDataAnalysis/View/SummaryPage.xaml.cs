using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using WeatherDataAnalysis.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherDataAnalysis.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummaryPage
    {
        private readonly SummaryViewModel summaryViewModel;
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SummaryPage" /> class.
        /// </summary>
        public SummaryPage()
        {
            this.InitializeComponent();
            this.summaryViewModel = new SummaryViewModel();
            
        }

        #endregion

        #region Methods

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        /// <summary>
        ///     Invoked when the Page is loaded and becomes the current source of a parent Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that can be examined by overriding code. The event data is representative of the pending
        ///     navigation that will load the current Page. Usually the most relevant property to examine is Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var param = (MainPage) e.Parameter;
            DataContext = this.summaryViewModel;
            this.radioButton10.IsChecked = true;
            this.summaryViewModel.WeatherCalculator = param?.ViewModel.WeatherCalculator;
            this.summaryViewModel.CreateSummary();
        }
      #endregion

      
    }
}