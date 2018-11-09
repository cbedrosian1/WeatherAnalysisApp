using System;
using Windows.UI.Xaml.Data;

namespace WeatherDataAnalysis.Converter
{
    /// <summary>
    ///     Converts integers to strings and back
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class IntConverterForString : IValueConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var temperature = (int) value;
            return temperature.ToString();
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var tempString = (string) value;
            var valueToReturn = 0;

            if (!string.IsNullOrEmpty(tempString))
            {
                valueToReturn = int.Parse(tempString);
            }

            return valueToReturn;
        }

        #endregion
    }
}