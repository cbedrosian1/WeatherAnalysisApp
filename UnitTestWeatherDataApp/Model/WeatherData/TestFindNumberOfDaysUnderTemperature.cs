using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    //  Input ({WeatherData.Days} in WeatherData)                                             Expected output
    //  [{1/3/2015,89,33,0}]                                                                         0
    //  [{1/2/2015,94,32,0}]                                                                         1
    //  [{1/1/2016,91,31,0}]                                                                         1
    //  [{1/4/2015,90,15,0}, {1/1/2015,91,31,0}]                                                     2
    //  [{1/2/2015,94,25,0}, {1/1/2015,91,15,0}]  (with incorrect year passed in)                    0
    //  [{}]                                                                                         0
    [TestClass]
    public class TestFindNumberOfDaysUnderTemperature
    {
        #region Data members

        private WeatherCalculator weatherData;
        private List<DailyStats> days;
        private DailyStats day1;
        private DailyStats day2;
        private DailyStats day3;
        private DailyStats day4;
        private DailyStats day5;

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInit()
        {
            this.days = new List<DailyStats>();
            this.day1 = new DailyStats(new DateTime(2015, 1, 1), 91, 31, 0);
            this.day2 = new DailyStats(new DateTime(2015, 1, 2), 94, 32, 0);
            this.day3 = new DailyStats(new DateTime(2015, 1, 3), 89, 33, 0);
            this.day4 = new DailyStats(new DateTime(2015, 1, 4), 90, 15, 0);
            this.day5 = new DailyStats(new DateTime(2016, 1, 1), 91, 15, 0);
        }

        [TestMethod]
        public void Test1MoreThanThresholdNotReturned()
        {
            this.days.Add(this.day3);
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysUnderTemperature(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestRightAtThresholdReturned()
        {
            this.days.Add(this.day2);
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(1, this.weatherData.FindNumberOfDaysUnderTemperature(this.day2.Date.Year));
        }

        [TestMethod]
        public void Test1LessThanThresholdReturned()
        {
            this.days.Add(this.day1);
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(1, this.weatherData.FindNumberOfDaysUnderTemperature(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestMultipleLessThanThresholdReturned()
        {
            this.days.Add(this.day4);
            this.days.Add(this.day1);
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(2, this.weatherData.FindNumberOfDaysUnderTemperature(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestWrongYearNotReturned()
        {
            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysUnderTemperature(this.day5.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherCalculator(this.days) {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysUnderTemperature(this.day1.Date.Year));
        }

        #endregion

    }
}