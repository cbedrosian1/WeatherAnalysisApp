using System;
using Windows.UI.Xaml.Data;
using Microsoft.Toolkit.Extensions;

namespace WeatherDataAnalysis.Converter
{
    /// <summary>
    ///     Converts integers to strings and back
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class DoubleConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns string of the double </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var precipitation = (double) value;
            return precipitation.ToSafeString();
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns double from a string</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var precipitationText = (string) value;

            var valueToReturn = 0.0;
            if (!string.IsNullOrEmpty(precipitationText))
            {
                valueToReturn = double.Parse(precipitationText);
            }

            return valueToReturn;
        }

        #endregion
    }
}