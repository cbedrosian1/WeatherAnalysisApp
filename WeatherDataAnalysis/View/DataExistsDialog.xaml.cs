using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherDataAnalysis.View
{
    /// <summary>
    ///     Options for button clicks in ContentDialog
    /// </summary>
    public enum MergeOrReplaceResult
    {
        /// <summary>
        ///     Used if the user clicks the merge button
        /// </summary>
        Merge,

        /// <summary>
        ///     Used if the user clicks the replace button
        /// </summary>
        Replace
    }

    /// <summary>
    ///     A ContentDialog to give the user the option of replacing existing text or merging with existing text
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.ContentDialog" />
    public sealed partial class DataExistsDialog
    {
        #region Properties

        /// <summary>
        ///     Stores the result of button clicks
        /// </summary>
        /// <value>
        ///     The result of button clicks.
        /// </value>
        public MergeOrReplaceResult Result { get; private set; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:WeatherDataAnalysis.View.DataExistsDialog" /> class.
        /// </summary>
        public DataExistsDialog()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        private void ContentDialog_MergeButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = MergeOrReplaceResult.Merge;
        }

        private void ContentDialog_ReplaceButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Result = MergeOrReplaceResult.Replace;
        }

        #endregion
    }
}