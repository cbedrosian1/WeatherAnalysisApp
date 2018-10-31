using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    [TestClass]
    public class TestFindNumberOfDaysOverTemperature
    {

        //  Input ({WeatherData.Days} in WeatherData)                                            Expected output
        //  [{1/3/2015,89,30}]                                                                         0
        //  [{1/4/2015,90,15}]                                                                         1
        //  [{1/1/2016, 91, 15}]                                                                       1
        //  [{1/2/2015,94,25}, {1/1/2015,91,15}]                                                       2
        //  [{1/2/2015,94,25}, {1/1/2015,91,15}]  (with incorrect year passed in)                      0
        //  [{}]                                                                                       0

        #region Data members

        private WeatherDataAnalysis.Model.WeatherData weatherData;
        private List<DailySummary> days;
        private DailySummary day1;
        private DailySummary day2;
        private DailySummary day3;
        private DailySummary day4;
        private DailySummary day5;

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInit()
        {
            this.days = new List<DailySummary>();
            this.day1 = new DailySummary(new DateTime(2015, 1, 1), 91, 15);
            this.day2 = new DailySummary(new DateTime(2015, 1, 2), 94, 25);
            this.day3 = new DailySummary(new DateTime(2015, 1, 3), 89, 30);
            this.day4 = new DailySummary(new DateTime(2015, 1, 4), 90, 15);
            this.day5 = new DailySummary(new DateTime(2016, 1, 1), 91, 15);
        }

        [TestMethod]
        public void Test1LessThanThresholdNotReturned()
        {
            this.days.Add(this.day3);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days) {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysOverTemperature(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestRightAtThresholdReturned()
        {
            this.days.Add(this.day4);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days) {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(1, this.weatherData.FindNumberOfDaysOverTemperature(this.day4.Date.Year));
        }

        [TestMethod]
        public void Test1MoreThanThresholdReturned()
        {
            this.days.Add(this.day5);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days) {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(1, this.weatherData.FindNumberOfDaysOverTemperature(this.day5.Date.Year));
        }

        [TestMethod]
        public void TestMultipleMoreThanThresholdReturned()
        {
            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days) {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(2, this.weatherData.FindNumberOfDaysOverTemperature(this.day2.Date.Year));
        }

        [TestMethod]
        public void TestWrongYearNotReturned()
        {
            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days) {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysOverTemperature(this.day5.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days)
            {
                HighTemperatureThreshold = 90
            };
            Assert.AreEqual(0, this.weatherData.FindNumberOfDaysOverTemperature(this.day1.Date.Year));
        }

        #endregion  
    }
}