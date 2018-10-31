using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    [TestClass]
    public class TestFindHighestLowTempDaysOfYear
    {

        //  Input ({WeatherData.Days} in WeatherData)                                            Expected output
        //  [{1/1/2015,50,15}]                                                                 [{1/1/2015,50,15}]
        //  [{1/3/2015,40,30}, {1/1/2015,50,15}, {1/2/2015,45,25}]                             [{1/3/2015,40,30}]
        //  [{1/2/2015,45,25}, {1/3/2015,40,30}, {1/1/2015,50,15}]                             [{1/3/2015,40,30}]
        //  [{1/2/2015,45,25}, {1/1/2015,50,15}, {1/3/2015,40,30}]                             [{1/1/2015, 50, 15}]
        //  [{1/1/2015,50,15}, {1/2/2015,45,25}, {1/3/2015,40,30}, {1/4/2015,50,30}]           [{1/3/2015,40,30}, {1/4/2015,50,30}]
        //  [{1/1/2015,50,15}, {1/1/2016, 50, 15}]                                             [{1/1/2015, 50, 15}]
        //  [{}]                                                                               InvalidExceptionError

        #region Data members

        private WeatherDataAnalysis.Model.WeatherData weatherData;
        private List<DailySummary> days;
        private List<DailySummary> testList;
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
            this.testList = new List<DailySummary>();
            this.day1 = new DailySummary(new DateTime(2015, 1, 1), 50, 15);
            this.day2 = new DailySummary(new DateTime(2015, 1, 2), 45, 25);
            this.day3 = new DailySummary(new DateTime(2015, 1, 3), 40, 30);
            this.day4 = new DailySummary(new DateTime(2015, 1, 4), 50, 30);
            this.day5 = new DailySummary(new DateTime(2016, 1, 1), 50, 15);
        }

        [TestMethod]
        public void TestOneDayInData()
        {
            this.days.Add(this.day1);
            this.testList.Add(this.day1);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestHighestLowDayIsFirst()
        {
            this.testList.Add(this.day3);

            this.days.Add(this.day3);
            this.days.Add(this.day1);
            this.days.Add(this.day2);

            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestHighestLowDayIsSecond()
        {
            this.testList.Add(this.day3);

            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.days.Add(this.day1);

            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestHighestLowDayIsLast()
        {
            this.testList.Add(this.day3);

            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.days.Add(this.day3);

            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestMultipleHighestLowDays()
        {
            this.testList.Add(this.day3);
            this.testList.Add(this.day4);

            this.days.Add(this.day1);
            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.days.Add(this.day4);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestDifferentYearNotReturned()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day1);
            this.days.Add(this.day5);
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherDataAnalysis.Model.WeatherData(this.days);
            Assert.ThrowsException<InvalidOperationException>(() =>
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day1.Date.Year));
        }



        #endregion
    }
}