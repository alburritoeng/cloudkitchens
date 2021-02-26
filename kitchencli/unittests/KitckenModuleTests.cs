using System;
using System.Threading;
using kitchencli;
using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace unittests.Properties
{
    [TestFixture]
    public class KitchenModuleTest
    {
        [Test]
        public void Stop_Test()
        {
            IStartStoppableModule kitchenModule = new KitchenModule(null);
            kitchenModule.Start();
            
            Assert.DoesNotThrow(()=>
            {
                kitchenModule.Stop();
            });
            
            Assert.NotNull(kitchenModule as IDisposable);
            Assert.DoesNotThrow(()=>{((IDisposable)kitchenModule).Dispose();});
        }

        [Test]
        public void PrepareOrder_Test()
        {
            Mock<ICourierOrderMatcher> courierOrderMatcherMock = new Mock<ICourierOrderMatcher>();
            Order order = new Order();
            order.prepTime = 1;
            
            IKitchen kitchen = new KitchenModule(courierOrderMatcherMock.Object);
            ((IStartStoppableModule)kitchen).Start();

            kitchen.PrepareOrder(order);
            Thread.Sleep(2 * 1000 );

            courierOrderMatcherMock.Verify(x=>x.AddToOrderReadyQueue(It.IsAny<Order>()), Times.Once);
            ((IStartStoppableModule)kitchen).Stop();
            ((IDisposable)kitchen).Dispose();
        }

        [Test]
        public void CourierHasArrived_Test()
        {
            Mock<ICourierOrderMatcher> courierOrderMatcherMock = new Mock<ICourierOrderMatcher>();
            ICourier courier = new DoorDashCourier();

            ((DoorDashCourier) courier)._durationEstimate = 1;

            IKitchen kitchen = new KitchenModule(courierOrderMatcherMock.Object);
            ((IStartStoppableModule)kitchen).Start();

            kitchen.CourierHasArrived(courier);
            Thread.Sleep(2 * 1000 );

            courierOrderMatcherMock.Verify(x=>x.CourierArrived(It.IsAny<ICourier>()), Times.Once);
            ((IStartStoppableModule)kitchen).Stop();
            ((IDisposable)kitchen).Dispose();
        }

        [Test]
        public void Kitchen_CancelledToken_Test()
        {
            Mock<ICourierOrderMatcher> courierOrderMatcherMock = new Mock<ICourierOrderMatcher>();
            ICourier courier = new DoorDashCourier();

            IKitchen kitchen = new KitchenModule(courierOrderMatcherMock.Object);
            
            ((KitchenModule)kitchen).cts.Cancel();
            
            ((IStartStoppableModule)kitchen).Start();
        }
    }
}