using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.CourierMatch
{
    /// <summary>
    /// Manages the order handoff to a Courier once it is finished
    /// This implementations finds an the first available courier and sets its OrderId to the next avaiable order
    /// when more than 1 order is available, an arbitrary order is assigned to the Courier
    /// </summary>
    internal class CourierOrderMatch : ICourierOrderMatcher
    {
        //public event Action<ICourier> CourierArrived;
        //public event Action<Order> OrderReady;

        public CourierOrderMatch()
        {
            //CourierArrived += CourierOrderMatch_CourierArrived;

            //OrderReady += CourierOrderMatch_OrderReady;
        }

        private void CourierOrderMatch_OrderReady(Order order)
        {
            Console.WriteLine($"Order Ready {order.id}\t{order.name}");
        }

        private void CourierOrderMatch_CourierArrived(ICourier courier)
        {
            Console.WriteLine($"Courier has arrived {courier.GetType()} for order {courier.OrderId()}");
        }

        public void AddToOrderReadyQueue(Order order)
        {
            throw new NotImplementedException();
        }

        public void CourierArrived(ICourier courier)
        {
            throw new NotImplementedException();
        }
    }
}
