using System;
using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using NUnit.Framework;

namespace unittests
{
    [TestFixture]
    public class CourierTests
    {
        [Test]
        public void Test1()
        {

        }

        [Test]
        public void TestDurationIsSet()
        {
            ICourier courier = new UberEatsCourier(new Order());
            Console.WriteLine($"Courier DurationEstimate = {courier.DurationEstimateInSeconds()}");
            Assert.IsTrue(courier.DurationEstimateInSeconds() != -1);
        }
    }
}
