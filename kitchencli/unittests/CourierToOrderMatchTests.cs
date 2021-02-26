using System;
using kitchencli.api;
using kitchencli.CourierMatch;
using kitchencli.Couriers;
using kitchencli.utils;
using Moq;
using NUnit.Framework;

namespace unittests
{
    [TestFixture]
    public class CourierToOrderMatchTests
    {
        [Test]
        public void AddToOrderReadyQueue_NullOrder_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);
            Assert.DoesNotThrow(() =>
            {
                match.AddToOrderReadyQueue(null);
            });
        }
        
        [Test]
        public void AddToOrderReadyQueue_ValidOrder_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);

            ICourier courier = new DoorDashCourier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            ((CourierOrderMatch) match)._matchingSet.Add(Guid.Parse(order.id),new Tuple<Order, ICourier>(null, courier));
            
            match.AddToOrderReadyQueue(order);
            
            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            Assert.True(((CourierOrderMatch) match)._matchingSet.Count == 0);
        }
        
        [Test]
        public void CourierArrived_NullCourier_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);

            Assert.DoesNotThrow(() =>
            {
                match.CourierArrived(null);
            });
        }

        
        [Test]
        public void CourierArrived_ValidCourier_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);

            ICourier courier = new DoorDashCourier();
            courier.CurrentOrder = order;
            Assert.AreEqual(order.id, courier.CurrentOrder.id);

            match.CourierArrived(courier);
            
            //testing that even though courier has a set Order, this is  and will take whatever order is ready
            Assert.True(((CourierOrderMatch) match)._matchingSet.ContainsKey(courier.OrderId()));

            match.AddToOrderReadyQueue(order);
            
            Assert.False(((CourierOrderMatch) match)._matchingSet.ContainsKey(courier.OrderId()));
        }
        
        [Test]
        public void CourierArrived_ValidCourier_NoOrderReady_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);

            ICourier courier = new DoorDashCourier();
            
            match.CourierArrived(courier);
            
            //Assert.True(((CourierOrderMatch) match)._idleCouriers.Count == 1);
        }

        [Test]
        public void ValidCourier_OrderReady_NotCorrectOrder_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher match = new CourierOrderMatch(courierPool.Object, telemetry.Object);

            ICourier courier = new DoorDashCourier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            match.AddToOrderReadyQueue(order);
            match.CourierArrived(courier);
            match.CourierArrived(new GrubHubCourier(){CurrentOrder = new Order()});
            match.CourierArrived(new DoorDashCourier(){CurrentOrder = new Order()});
            match.CourierArrived(new UberEatsCourier(){CurrentOrder = new Order()});
            
            //order should just sit there. 
            Assert.True(((CourierOrderMatch) match)._matchingSet.ContainsKey(Guid.Parse(order.id)));
        }
        
        [Test]
        public void GetMatchType_Test()
        {
            ICourierOrderMatcher match = new CourierOrderMatch(null, null);
            Assert.True(match.GetMatchType() == MatchType.Match);

        }
    }
}