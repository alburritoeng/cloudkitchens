using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.api
{
    /// <summary>
    /// This interface receives Orders and handles logic for 
    /// Couriers picking up orders
    /// 
    /// Concrete implementations of this interface can implement Match or First-in-First-Out logic for 
    /// order hand off to Couriers
    /// </summary>
    public interface ICourierOrderMatcher
    {
        void AddToOrderReadyQueue(Order order);

        void CourierArrived(ICourier courier);
    }
}
