using System;
using System.Collections.Generic;
using iEnvironment.Domain.Models;
using Xunit;

namespace DomainTests
{
    public class EventTest
    {
        [Fact]
        public void ShouldRunTestStartBeforeEnd()
        {
            var eventToTest = new Event
            {
                StartTime = "12:00",
                EndTime = "15:00",
                RunningDays = new List<DayOfWeek>{ (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = true

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 14:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);
            Assert.True(eventToTest.ShouldRun(datetime));
        }

        [Fact]
        public void DontShouldRunTestStartBeforeEnd()
        {
            var eventToTest = new Event
            {
                StartTime = "12:00",
                EndTime = "15:00",
                RunningDays = new List<DayOfWeek> { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = true

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 10:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);



            Assert.False(eventToTest.ShouldRun(datetime));


        }


        [Fact]
        public void ShouldRunTestStartAfterEnd()
        {
            var eventToTest = new Event
            {
                StartTime = "15:00",
                EndTime = "12:00",
                RunningDays = new List<DayOfWeek> { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = true

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 10:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);
            Assert.True(eventToTest.ShouldRun(datetime));
        }


        [Fact]
        public void DontShouldRunTestStartAfterEnd()
        {
            var eventToTest = new Event
            {
                StartTime = "15:00",
                EndTime = "12:00",
                RunningDays = new List<DayOfWeek> { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = true

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 14:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);



            Assert.False(eventToTest.ShouldRun(datetime));


        }

        


        [Fact]
        public void DontShouldRunTestStartBeforeEndWithoutRunningDays()
        {
            var eventToTest = new Event
            {
                StartTime = "12:00",
                EndTime = "15:00",
                //RunningDays = new List<DayOfWeek> { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = true

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 14:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);



            Assert.False(eventToTest.ShouldRun(datetime));


        }

        [Fact]
        public void DontShouldRunNotTimeBaseds()
        {
            var eventToTest = new Event
            {
                StartTime = "12:00",
                EndTime = "15:00",
                RunningDays = new List<DayOfWeek> { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 },
                TimeBased = false

            };

            Console.WriteLine(DateTime.Now);

            var datetime = DateTime.ParseExact("2021-09-26 14:40", "yyyy-MM-dd HH:mm",
                                       System.Globalization.CultureInfo.InvariantCulture);

            Assert.False(eventToTest.ShouldRun(datetime));

        }
    }
}
