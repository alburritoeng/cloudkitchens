using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli
{
    /// <summary>
    /// The purpose of the class is to receive an order from the Cli
    /// The order is in the JSON format specified
    /// A concret order is created from the JSON 
    /// A courier is dispatched for the order
    /// An Order object is dispatched to the IFoodOrderStatusManager object
    /// </summary>
    internal class OrderReceiverModule : IOrderReceiver
    {
        IFoodOrderMaker _foodOrderMaker;
        ICourierFactory _courierFactory;
        public OrderReceiverModule(IFoodOrderMaker foodOrderMaker, ICourierFactory courierFactory)
        {
            _foodOrderMaker = foodOrderMaker;
            _courierFactory = courierFactory;
        }

        public void DispatchCourier(Order order)
        {
            ICourier courier = _courierFactory.CreateCourier(order);
            //courierOrderMatcher.CourierArrived += courier.NotifyHereForOrder;
            Console.WriteLine($"Courier from {courier.CourierType()} on the way for order {order.id}\t{order.name}");

        }

        public void SendOrderToOrderMaker(Order order)
        {
            Console.WriteLine($"Sending order {order.id}\r{order.name} to OrderMaker");
            
            
        }
    }
}
