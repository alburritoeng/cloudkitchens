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
    /// This implementations finds an exact Order Id to match against a courier's Order Id
    /// </summary>
    internal class CourierOrderFifo : ICourierOrderMatcher
    {
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
