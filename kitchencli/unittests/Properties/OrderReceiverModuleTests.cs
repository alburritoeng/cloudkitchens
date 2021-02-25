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
            Mock<IFoodOrderMaker> foodMakerMock = new Mock<IFoodOrderMaker>();
            
            Mock<ICourierFactory> courierFactoryMock = new Mock<ICourierFactory>();
            
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
            Mock<IFoodOrderMaker> foodMakerMock = new Mock<IFoodOrderMaker>();
            
            Mock<ICourierFactory> courierFactoryMock = new Mock<ICourierFactory>();
            
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
            Mock<IFoodOrderMaker> foodMakerMock = new Mock<IFoodOrderMaker>();
            Order order = new Order();

            Mock<ICourier> courierMock = new Mock<ICourier>();
            
            Mock<ICourierFactory> courierFactoryMock = new Mock<ICourierFactory>();
            courierFactoryMock.Setup(x => x.CreateCourier(order)).Returns(courierMock.Object);
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
            Mock<IFoodOrderMaker> foodMakerMock = new Mock<IFoodOrderMaker>();
            Order order = new Order();

            Courier courier = new DoorDashCourier(null);
            
            Mock<ICourierFactory> courierFactoryMock = new Mock<ICourierFactory>();
            courierFactoryMock.Setup(x => x.CreateCourier(order)).Returns(courier);
            
            Mock<ICourierOrderMatcher> courierOrderMaker = new Mock<ICourierOrderMatcher>();
            courierOrderMaker.Setup(x => x.GetMatchType()).Returns(MatchType.Fifo);
            
            IOrderReceiver orderReceiver = new OrderReceiverModule(foodMakerMock.Object, courierFactoryMock.Object,courierOrderMaker.Object);
            ((IStartStoppableModule)orderReceiver).Start();

            orderReceiver.DispatchCourier(order);
            
            Thread.Sleep(700);
            courierMock.Verify(cour);

            ((IStartStoppableModule)orderReceiver).Stop();
        }

        [Test]
        public void DispatchCourier_CurrentOrder_Match_Test()
        {
            
        }
    }
}