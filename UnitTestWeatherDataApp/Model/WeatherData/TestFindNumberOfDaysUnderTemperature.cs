using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    [TestClass]
    public class TestFindNumberOfDaysUnderTemperature
    {
        //  Input ({WeatherData.Days} in WeatherData)                                             Expected output
        //  [{1/3/2015,89,33,0}]                                                                         0
        //  [{1/2/2015,94,32,0}]                                                                         1
        //  [{1/1/2016,91,31,0}]                                                                         1
        //  [{1/4/2015,90,15,0}, {1/1/2015,91,31,0}]                                                     2
        //  [{1/2/2015,94,25,0}, {1/1/2015,91,15,0}]  (with incorrect year passed in)                    0
        //  [{}]                                                                                         0

        #region Data members

        private WeatherCalculator weatherData;
        private List<DailyStats> days;
        private DailyStats day1;
        private DailyStats day2;
        private DailyStats day3;
        private DailyStats day4;
        private DailyStats day5;

        #endregion


        [TestInitialize]
        public void TestInit()
        {
            days = new List<DailyStats>();
            day1 = new DailyStats(new DateTime(2015, 1, 1), 91, 31, 0);
            day2 = new DailyStats(new DateTime(2015, 1, 2), 94, 32, 0);
            day3 = new DailyStats(new DateTime(2015, 1, 3), 89, 33, 0);
            day4 = new DailyStats(new DateTime(2015, 1, 4), 90, 15, 0);
            day5 = new DailyStats(new DateTime(2016, 1, 1), 91, 15, 0);
        }

        [TestMethod]
        public void Test1MoreThanThresholdNotReturned()
        {
            days.Add(day3);
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, weatherData.FindNumberOfDaysUnderTemperature(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestRightAtThresholdReturned()
        {
            days.Add(day2);
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(1, weatherData.FindNumberOfDaysUnderTemperature(day2.Date.Year));
        }

        [TestMethod]
        public void Test1LessThanThresholdReturned()
        {
            days.Add(day1);
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(1, weatherData.FindNumberOfDaysUnderTemperature(day1.Date.Year));
        }

        [TestMethod]
        public void TestMultipleLessThanThresholdReturned()
        {
            days.Add(day4);
            days.Add(day1);
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(2, weatherData.FindNumberOfDaysUnderTemperature(day1.Date.Year));
        }

        [TestMethod]
        public void TestWrongYearNotReturned()
        {
            days.Add(day2);
            days.Add(day1);
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, weatherData.FindNumberOfDaysUnderTemperature(day5.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            weatherData = new WeatherCalculator(days)
            {
                LowTemperatureThreshold = 32
            };
            Assert.AreEqual(0, weatherData.FindNumberOfDaysUnderTemperature(day1.Date.Year));
        }
     
    }
}