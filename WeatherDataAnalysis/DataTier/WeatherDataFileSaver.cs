using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.DataTier
{
    /// <summary>
    ///     Puts existing weather data into a csv file
    /// </summary>
    public class WeatherDataFileSaver
    {
        #region Methods

        /// <summary>
        ///     Saves the collection of data into a csv file
        ///     Precondition: daySummaries != null
        /// </summary>
        /// <param name="daySummaries">The data being saved</param>
        public async void SaveFile(ICollection<DailyStats> daySummaries, StorageFile file)
        {
            if (daySummaries == null)
            {
                throw new ArgumentNullException(nameof(daySummaries));
            }

            if (file.FileType == ".csv")
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, this.formatToCSV(daySummaries));
            }

            if (file.FileType == ".xml")
            {
                CachedFileManager.DeferUpdates(file);
                await this.formatToXML(daySummaries, file);
            }
        }

        private async Task formatToXML(ICollection<DailyStats> daySummaries, StorageFile file)
        {
            var serializer = new XmlSerializer(typeof(List<DailyStats>));
            var writer = await file.OpenStreamForWriteAsync();
            serializer.Serialize(writer, daySummaries);
        }

        private string formatToCSV(ICollection<DailyStats> dailyStats)
        {
            var days = dailyStats.OrderBy(day => day.Date);
            var csv = new StringBuilder();
            foreach (var day in days)
            {
                csv.AppendJoin(',', day.Date.ToShortDateString(), day.HighTemperature, day.LowTemperature);
                csv.Append(Environment.NewLine);
            }

            return csv.ToString();
        }

        #endregion
    }
}