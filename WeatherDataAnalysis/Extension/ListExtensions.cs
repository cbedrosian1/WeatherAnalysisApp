using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace WeatherDataAnalysis.Extension
{
    /// <summary>
    /// Extends the list class in order to convert a list to an observable collection
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Converts a list to the observable collection.
        /// </summary>
        /// <typeparam name="T"> the object type being inputted</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>the collection converted</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }
    }
}
