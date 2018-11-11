using System;
using Windows.UI.Xaml.Data;

namespace WeatherDataAnalysis.Converter
{
    /// <summary>
    ///     Converts integers to datetime and back
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class IntConverterForDateTime : IValueConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns string from int</returns>
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
        /// <returns>Returns int from DateTime</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var tempDate = DateTime.Now;
            if (value != null)
            {
                tempDate = (DateTime) value;
            }

            var valueToReturn = tempDate.Year;

            return valueToReturn;
        }

        #endregion
    }
}