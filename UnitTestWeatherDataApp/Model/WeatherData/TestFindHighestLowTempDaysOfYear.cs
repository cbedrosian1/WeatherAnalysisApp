using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    [TestClass]
    public class TestFindHighestLowTempDaysOfYear
    {
        //  Input ({WeatherData.Days} in WeatherData)                                                 Expected output
        //  [{1/1/2015,50,15,0}]                                                                     [{1/1/2015,50,15,0}]
        //  [{1/3/2015,40,30,0}, {1/1/2015,50,15,0}, {1/2/2015,45,25,0}]                             [{1/3/2015,40,30,0}]
        //  [{1/2/2015,45,25,0}, {1/3/2015,40,30,0}, {1/1/2015,50,15,0}]                             [{1/3/2015,40,30,0}]
        //  [{1/2/2015,45,25,0}, {1/1/2015,50,15,0}, {1/3/2015,40,30,0}]                             [{1/1/2015, 50, 15,0}]
        //  [{1/1/2015,50,15,0}, {1/2/2015,45,25,0}, {1/3/2015,40,30,0}, {1/4/2015,50,30,0}]         [{1/3/2015,40,30,0}, {1/4/2015,50,30,0}]
        //  [{1/1/2015,50,15,0}, {1/1/2016, 50, 15,0}]                                               [{1/1/2015, 50, 15,0}]
        //  [{}]                                                                                     InvalidExceptionError

        #region Data members

        private WeatherCalculator weatherData;
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
            this.day1 = new DailyStats(new DateTime(2015, 1, 1), 50, 15, 0);
            this.day2 = new DailyStats(new DateTime(2015, 1, 2), 45, 25, 0);
            this.day3 = new DailyStats(new DateTime(2015, 1, 3), 40, 30, 0);
            this.day4 = new DailyStats(new DateTime(2015, 1, 4), 50, 30, 0);
            this.day5 = new DailyStats(new DateTime(2016, 1, 1), 50, 15, 0);
        }

        [TestMethod]
        public void TestOneDayInData()
        {
            this.days.Add(this.day1);
            this.testList.Add(this.day1);
            this.weatherData = new WeatherCalculator(this.days);
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

            this.weatherData = new WeatherCalculator(this.days);
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

            this.weatherData = new WeatherCalculator(this.days);
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

            this.weatherData = new WeatherCalculator(this.days);
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
            this.weatherData = new WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day3.Date.Year));
        }

        [TestMethod]
        public void TestDifferentYearNotReturned()
        {
            this.testList.Add(this.day1);

            this.days.Add(this.day1);
            this.days.Add(this.day5);
            this.weatherData = new WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day1.Date.Year));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherCalculator(this.days);
            Assert.ThrowsException<InvalidOperationException>(() =>
                this.weatherData.FindHighestLowTemperatureDaysOfYear(this.day1.Date.Year));
        }

        #endregion

       
    }
}