using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using WeatherDataAnalysis.Model;

namespace WeatherDataAnalysis.DataTier
{
    /// <summary>
    ///     Reads a file and creates Day objects
    /// </summary>
    public class WeatherDataCsvParser
    {
        #region Data members

        private const int DateField = 0;
        private const int HighTempField = 1;
        private const int LowTempField = 2;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the errors in the file being read.
        /// </summary>
        /// <value>
        ///     The errors in the file being read.
        /// </value>
        public string LinesWithErrors { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Loads and reads the file.
        ///     Precondition: file != null
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>
        ///     List of days from file being read
        /// </returns>
        /// <exception cref="ArgumentNullException">file</exception>
        public async Task<ICollection<DailyStats>> LoadFile(StorageFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.LinesWithErrors = string.Empty;
            var stream = await file.OpenStreamForReadAsync();

            using (var reader = new StreamReader(stream))
            {
                var days = this.parseCsvFile(reader);
                return days;
            }
        }

        private ICollection<DailyStats> parseCsvFile(StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var days = new List<DailyStats>();
            var lineCount = 0;
            var lineWithError = string.Empty;
            while (!reader.EndOfStream)
            {
                try
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        lineCount++;
                        lineWithError = line;

                        var values = line.Split(',');
                        var date = DateTime.Parse(values[DateField]);
                        var highTemp = int.Parse(values[HighTempField]);
                        var lowTemp = int.Parse(values[LowTempField]);
                        var day = new DailyStats(date, highTemp, lowTemp);
                        days.Add(day);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    this.LinesWithErrors += $"Error: {lineWithError}     Line: {lineCount}" + Environment.NewLine;
                }
            }

            return days;
        }

        #endregion
    }
}