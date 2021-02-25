using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.api
{
    /// <summary>
    /// The purpose of this api is to receive an Order, 
    /// add the order to the order queue and send complete orders to "the pass" (ICourierOrderMatcher)
    /// pickup by Couriers
    /// 
    /// IWO this interface is to manage the order's life cycle after it is received from the cli/json/order-originator
    /// </summary>
    interface IFoodOrderMaker
    {
        void PrepareOrder(Order order);
    }
}
