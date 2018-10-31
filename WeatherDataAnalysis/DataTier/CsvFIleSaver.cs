using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.DataTier
{
    /// <summary>
    ///     Puts existing weather data into a csv file
    /// </summary>
    public class CsvFIleSaver
    {
        #region Methods

        /// <summary>
        ///     Saves the collection of data into a csv file
        ///     Precondition: daySummaries != null
        /// </summary>
        /// <param name="daySummaries">The data being saved</param>
        public async void SaveFile(ICollection<DailySummary> daySummaries)
        {
            if (daySummaries == null)
            {
                throw new ArgumentNullException(nameof(daySummaries));
            }

            var days = daySummaries.OrderBy(day => day.Date);
            var csv = new StringBuilder();
            foreach (var day in days)
            {
                csv.AppendJoin(',', day.Date.ToShortDateString(), day.HighTemperature, day.LowTemperature);
                csv.Append(Environment.NewLine);
            }

            var savePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("CSV", new List<string> {".csv"});
            savePicker.SuggestedFileName = "New Document";
            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, csv.ToString());
            }
        }

        #endregion
    }
}