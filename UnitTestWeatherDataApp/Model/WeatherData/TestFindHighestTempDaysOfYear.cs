using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    [TestClass]
    public class TestFindHighestTempDaysOfYear
    {

        //  Input ({WeatherData.Days} in WeatherData)                                                Expected output
        //  [{1/1/2015,50,15,1}]                                                                     [{1/1/2015, 50, 15}]
        //  [{1/1/2015,50,15,1}, {1/2/2015,45,25,1}, {1/3/2015,40,30,1}]                             [{1/1/2015, 50, 15}]
        //  [{1/2/2015,45,25,1}, {1/1/2015,50,15,1}, {1/3/2015,40,30,1}]                             [{1/1/2015, 50, 15}]
        //  [{1/2/2015,45,25,1}, {1/3/2015,40,30,1}, {1/1/2015,50,15,1}]                             [{1/1/2015, 50, 15}]
        //  [{1/1/2015,50,15,1}, {1/2/2015,45,25,1}, {1/3/2015,40,20,1}, {1/4/2015,50,15,1}]         [{1/1/2015, 50, 15}, {1/4/2015,50,15}]
        //  [{1/1/2015,50,15,1}, {1/1/2016, 50, 15,1}]                                               [{1/1/2015, 50, 15}]
        //  [{}]                                                                                     InvalidOperationException

        #region Data members

        private WeatherDataAnalysis.Model.WeatherCalculator weatherData;
        private List<DailyStats> days;
        private List<DailyStats> testList;
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
            this.testList = new List<DailyStats>();
            this.day1 = new DailyStats(new DateTime(2015, 1, 1), 50, 15, 1);
            this.day2 = new DailyStats(new DateTime(2015, 1, 2), 45, 25, 1);
            this.day3 = new DailyStats(new DateTime(2015, 1, 3), 40, 30, 1);
            this.day4 = new DailyStats(new DateTime(2015, 1, 4), 50, 15, 1);
            this.day5 = new DailyStats(new DateTime(2016, 1, 1), 50, 15, 1);
        }

        [TestMethod]
        public void TestOneDayInData()
        {
            this.days.Add(this.day1);
            this.testList.Add(this.day1);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestHighestDayIsFirst()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day1);
            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestHighestDayIsSecond()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.days.Add(this.day3);

            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestHighestDayIsLast()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.days.Add(this.day1);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestMultipleHighestDays()
        {
            this.testList.Add(this.day1);
            this.testList.Add(this.day4);

            this.days.Add(this.day1);
            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.days.Add(this.day4);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestDifferentYearNotReturned()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day1);
            this.days.Add(this.day5);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherDataAnalysis.Model.WeatherCalculator(this.days);
            Assert.ThrowsException<InvalidOperationException>(() =>
                this.weatherData.FindHighestTemperatureDaysOfYear(this.day1.Date.Year));
        }

        #endregion
    }
}