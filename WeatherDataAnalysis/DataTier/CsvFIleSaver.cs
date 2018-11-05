using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Pickers;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.DataTier
{
    /// <summary>
    ///     Puts existing weather data into a csv file
    /// </summary>
    public class CsvFileSaver
    {
        #region Methods

        /// <summary>
        ///     Saves the collection of data into a csv file
        ///     Precondition: daySummaries != null
        /// </summary>
        /// <param name="daySummaries">The data being saved</param>
        public async void SaveFile(ICollection<DailyStats> daySummaries)
        {
            if (daySummaries == null)
            {
                throw new ArgumentNullException(nameof(daySummaries));
            }

            

            var savePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("CSV", new List<string> {".csv"});
            savePicker.FileTypeChoices.Add("XML", new List<string>{".xml"});
            savePicker.SuggestedFileName = "New Document";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                if (file.FileType == ".csv")
                {
                    var csv = this.buildCsv(daySummaries);
                    CachedFileManager.DeferUpdates(file);
                    await FileIO.WriteTextAsync(file, csv);
                } else if (file.FileType == ".xml")
                {
                    var serializer = new XmlSerializer(typeof(List<DailyStats>));
                    var writer = await file.OpenStreamForWriteAsync();
                    serializer.Serialize(writer, daySummaries);
                    writer.Close();

                }
            


                
            }
        }

        private string buildCsv(ICollection<DailyStats> daySummaries)
        {
            var days = daySummaries.OrderBy(day => day.Date);
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