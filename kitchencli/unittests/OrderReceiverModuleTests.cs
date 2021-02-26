using System;
using System.Diagnostics;
using System.Threading;
using kitchencli;
using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using Moq;
using NUnit.Framework;

namespace unittests.Properties
{
    [TestFixture]
    public class OrderReceiverModuleTests
    {
        [Test]
        public void DispatchCourier_NullOrder_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();
            
            Order order = null;
            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(500);
            foodMakerMock.Verify(x => x.CourierHasArrived(It.IsAny<ICourier>()), Times.Never);
            
            ((IStartStoppableModule)orderReceiver).Stop();
        }
        
        [Test]
        public void DispatchCourier_ValidOrder_NoFactory_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();
            
            Order order = new Order();
            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(1200);
            foodMakerMock.Verify(x => x.CourierHasArrived(It.IsAny<ICourier>()), Times.Never);
            
            ((IStartStoppableModule)orderReceiver).Stop();
        }
        
        [Test]
        public void DispatchCourier_ValidOrder_FactoryValid_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Order order = new Order();

            Mock<ICourier> courierMock = new Mock<ICourier>();
            
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            courierFactoryMock.Setup(x => x.GetCourier()).Returns(courierMock.Object);
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(700);
            courierMock.Verify(x => x.LeaveForFood(), Times.Once);

            ((IStartStoppableModule)orderReceiver).Stop();
        }
        
        [Test]
        public void DispatchCourier_CurrentOrder_FIFO_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Order order = new Order();

            Mock<ICourier> courierMock = new Mock<ICourier>();

            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            courierFactoryMock.Setup(x => x.GetCourier()).Returns(courierMock.Object);
            
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();
            courierOrderMaker.Setup(x => x.GetMatchType()).Returns(MatchType.Fifo);
            
            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(700);
            
            courierMock.VerifySet(x => x.CurrentOrder = null);
            ((IStartStoppableModule)orderReceiver).Stop();
        }

        [Test]
        public void DispatchCourier_CurrentOrder_Match_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Order order = new Order();

            Mock<ICourier> courierMock = new Mock<ICourier>();
            
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            courierFactoryMock.Setup(x => x.GetCourier()).Returns(courierMock.Object);
            
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();
            courierOrderMaker.Setup(x => x.GetMatchType()).Returns(MatchType.Match);
            
            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(700);
            courierMock.Verify(x=>x.CurrentOrder, Times.Once);
            courierMock.VerifySet(x => x.CurrentOrder = order);
            ((IStartStoppableModule)orderReceiver).Stop();
        }

        [Test]
        public void SendOrderToOrderMaker_CancelledToken_Test()
        {
            //IKitchenIKitchen
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            Order order = new Order();
            
            OrderReceiverModule orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.cts.Cancel();
            orderReceiver.SendOrderToOrderMaker(order);
            Assert.True(orderReceiver._receivedOrdersQueue.Count == 1);
            orderReceiver.Stop();
        }
        
        [Test]
        public void SendOrderToOrderMaker_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            Order order = new Order();
            
            OrderReceiverModule orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.SendOrderToOrderMaker(order);
            Thread.Sleep(200);
            Assert.True(orderReceiver._receivedOrdersQueue.Count == 0);
            orderReceiver.Stop();
        }

        [Test]
        public void Stop_Test()
        {
            Mock<IKitchen> foodMakerMock = new Mock<IKitchen>();
            Mock<ICourierPool> courierFactoryMock = new Mock<ICourierPool>();
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();

            IStartStoppableModule module = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            module.Start();
            
            Assert.DoesNotThrow(()=>{module.Stop();});
            
            Assert.NotNull(module as IDisposable);
            Assert.DoesNotThrow(()=>{((IDisposable)module).Dispose();});
        }
    }
}