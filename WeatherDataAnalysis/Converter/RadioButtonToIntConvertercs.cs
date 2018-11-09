using System;
using Windows.UI.Xaml.Data;

namespace WeatherDataAnalysis.Converter
{
    public class RadioButtonToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }
}
