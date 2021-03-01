using kitchencli.api;
using kitchencli.CourierMatch;
using kitchencli.Couriers;
using kitchencli.utils;
using Moq;
using NUnit.Framework;

namespace unittests
{
    [TestFixture]
    public class CourierMatchFifo_Tests
    {
        /*
         *  void AddToOrderReadyQueue(Order order);

        void CourierArrived(ICourier courier);

        MatchType GetMatchType();
         */
        [Test]
        public void AddToOrderReadyQueue_NullOrder_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);
            Assert.DoesNotThrow(() =>
            {
                fifo.AddToOrderReadyQueue(null);
            });
        }
        
        [Test]
        public void AddToOrderReadyQueue_ValidOrder_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);

            ICourier courier = new Courier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            ((CourierOrderFifo) fifo)._idleCouriers.Enqueue(courier);
            
            fifo.AddToOrderReadyQueue(order);
            
            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            Assert.True(((CourierOrderFifo) fifo)._readyForPickupOrders.Count == 0);
        }
        
        [Test]
        public void CourierArrived_NullCourier_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);

            ICourier courier = new Courier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            ((CourierOrderFifo) fifo)._readyForPickupOrders.Add(order);

            Assert.DoesNotThrow(() =>
            {
                fifo.CourierArrived(null);
            });

            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            //Assert.True(((CourierOrderFifo) fifo)._readyForPickupOrders.Count == 0);
        }

        
        [Test]
        public void CourierArrived_ValidCourier_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);

            ICourier courier = new Courier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            ((CourierOrderFifo) fifo)._readyForPickupOrders.Add(order);
            
            fifo.CourierArrived(courier);
            
            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            Assert.True(((CourierOrderFifo) fifo)._readyForPickupOrders.Count == 0);
        }
        
        [Test]
        public void CourierArrived_ValidCourier_NoOrderReady_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);

            ICourier courier = new Courier();
            
            fifo.CourierArrived(courier);
            
            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            Assert.True(((CourierOrderFifo) fifo)._idleCouriers.Count == 1);
        }

        [Test]
        public void NoCourierArrived_ValidCourier_OrderReady_Test()
        {
            Mock<ICourierPool> courierPool = new Mock<ICourierPool>();
            Mock<ICourierOrderTelemetry> telemetry = new Mock<ICourierOrderTelemetry>();

            Order order = new Order();
            ICourierOrderMatcher fifo = new CourierOrderFifo(courierPool.Object, telemetry.Object);

            ICourier courier = new Courier();
            courier.CurrentOrder = new Order();
            Assert.AreNotEqual(order.id, courier.CurrentOrder.id);

            fifo.AddToOrderReadyQueue(order);
            
            //testing that even though courier has a set Order, this is FIFO and will take whatever order is ready
            Assert.True(((CourierOrderFifo) fifo)._readyForPickupOrders.Count == 1);
        }
        
        [Test]
        public void GetMatchType_Test()
        {
            ICourierOrderMatcher fifo = new CourierOrderFifo(null, null);
            Assert.True(fifo.GetMatchType() == MatchType.Fifo);

        }
    }
}