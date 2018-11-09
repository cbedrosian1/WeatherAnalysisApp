using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherDataAnalysis.View
{
    /// <summary>
    ///     Creates a dialog box for when duplicate days exist
    /// </summary>
    /// <seealso cref="ContentDialog" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector" />
    /// <seealso cref="Windows.UI.Xaml.Markup.IComponentConnector2" />
    public sealed partial class DuplicateDayDialog
    {
        #region Properties

        /// <summary>
        ///     Gets the result of the duplicate day buttons
        /// </summary>
        /// <value>
        ///     The result of the duplicate day buttons.
        /// </value>
        public DuplicateDayResult Result { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DuplicateDayDialog" /> class.
        /// </summary>
        /// <param name="text">The text to be displayed in TextBlock.</param>
        public DuplicateDayDialog(string text)
        {
            this.InitializeComponent();
            this.conflictingDaysTextBlock.Text = text;
        }

        #endregion

        #region Methods

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = new DuplicateDayResult {
                KeepOrReplace = KeepOrReplace.Keep,
                DoForAll = this.duplicateDayCheckBox.IsChecked ?? false
            };
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = new DuplicateDayResult {
                KeepOrReplace = KeepOrReplace.Replace,
                DoForAll = this.duplicateDayCheckBox.IsChecked ?? false
            };
        }

        #endregion
    }
}