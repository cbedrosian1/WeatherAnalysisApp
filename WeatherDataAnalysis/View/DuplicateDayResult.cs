namespace WeatherDataAnalysis.View
{
    /// <summary>
    ///     Options for the keep and replace button in ContentDialog
    /// </summary>
    public enum KeepOrReplace
    {
        /// <summary>
        ///     Used if user chooses Keep
        /// </summary>
        Keep,

        /// <summary>
        ///     Used if user chooses Replace
        /// </summary>
        Replace
    }

    /// <summary>
    ///     Keeps track of the result of the ContentDialog for duplicate days
    /// </summary>
    public class DuplicateDayResult
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the keep or replace enum.
        /// </summary>
        /// <value>
        ///     The keep or replace enum.
        /// </value>
        public KeepOrReplace KeepOrReplace { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the DoForAll checkbox is checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if DoForAll checkbox is checked; otherwise, <c>false</c>.
        /// </value>
        public bool DoForAll { get; set; }

        #endregion
    }
}