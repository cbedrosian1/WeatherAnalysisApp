using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDataAnalysis.Model;

namespace UnitTestWeatherDataApp.Model.WeatherData
{
    //  Input ({WeatherData.Days} in WeatherData)                                                   Expected output
    //  [{1/1/2015,50,15,1}]                                                                        [{1/1/2015,50,15,1}]
    //  [{1/1/2015,50,15,1}, {1/1/2016,45,25,1}]                                                    [1/1/2015,50,15,1},{1/1/2016,45,25,1}]
    //  [{1/1/2015,50,15,1}, {1/1/2016,45,25,1}, {1/1/2017,40,20,1}, {1/1/2018,50,15,1}]            [{1/1/2015,50,15,1},{1/1/2016,45,25,1},{1/1/2017,40,20,1},{1/1/2018,50,15,1}]
    //  2x[{1/1/2015,50,15,1}, 2x{1/1/2016,45,25,1}, 2x{1/1/2017,40,20,1}, 2x{1/1/2018,50,15,1}]    [{1/1/2015,50,15,1},{1/1/2016,45,25,1},{1/1/2017,40,20,1},{1/1/2018,50,15,1}]
    //  [{}]                                                                                        empty list
    [TestClass]
    public class TestFindYears
    {
        #region Data members

        private WeatherCalculator weatherData;
        private List<DailyStats> days;
        private List<DateTime> testList;
        private DailyStats day1;
        private DailyStats day2;
        private DailyStats day3;
        private DailyStats day4;

        #endregion

        #region Methods

        [TestInitialize]
        public void TestInit()
        {
            this.days = new List<DailyStats>();
            this.testList = new List<DateTime>();
            this.day1 = new DailyStats(new DateTime(2015, 1, 1), 50, 15, 1);
            this.day2 = new DailyStats(new DateTime(2016, 1, 1), 45, 25, 1);
            this.day3 = new DailyStats(new DateTime(2017, 1, 1), 40, 30, 1);
            this.day4 = new DailyStats(new DateTime(2018, 1, 1), 50, 15, 1);
        }

        [TestMethod]
        public void TestOneYearInData()
        {
            this.days.Add(this.day1);
            this.testList.Add(this.day1.Date);
            this.weatherData = new WeatherCalculator(this.days);
            var years = this.weatherData.FindYears();
            CollectionAssert.AreEquivalent(this.testList,
                years.ToList());
        }

        [TestMethod]
        public void TestTwoYears()
        {
            this.testList.Add(this.day1.Date);
            this.testList.Add(this.day2.Date);

            this.days.Add(this.day1);
            this.days.Add(this.day2);
            this.weatherData = new WeatherCalculator(this.days);
            var years = this.weatherData.FindYears();
            CollectionAssert.AreEquivalent(this.testList,
                years.ToList());
        }

        [TestMethod]
        public void TestMultipleYears()
        {
            this.testList.Add(this.day1.Date);
            this.testList.Add(this.day2.Date);
            this.testList.Add(this.day3.Date);
            this.testList.Add(this.day4.Date);

            this.days.Add(this.day1);
            this.days.Add(this.day2);
            this.days.Add(this.day3);
            this.days.Add(this.day4);
            this.weatherData = new WeatherCalculator(this.days);
            var years = this.weatherData.FindYears();
            CollectionAssert.AreEquivalent(this.testList,
                years.ToList());
        }

        [TestMethod]
        public void TestMultipleSameYears()
        {
            this.testList.Add(this.day1.Date.Date);
            this.testList.Add(this.day2.Date.Date);
            this.testList.Add(this.day3.Date.Date);
            this.testList.Add(this.day4.Date.Date);

            this.days.Add(this.day4);
            this.days.Add(this.day1);
            this.days.Add(this.day3);
            this.days.Add(this.day2);
            this.days.Add(this.day1);
            this.days.Add(this.day3);
            this.days.Add(this.day2);
            this.days.Add(this.day4);
            this.weatherData = new WeatherCalculator(this.days);
            var years = this.weatherData.FindYears();
            CollectionAssert.AreEquivalent(this.testList,
                years.ToList());
        }

        [TestMethod]
        public void TestEmptyList()
        {
            this.weatherData = new WeatherCalculator(this.days);
            CollectionAssert.AreEquivalent(this.testList,
                this.weatherData.FindYears().ToList());
        }

        #endregion

    }
}