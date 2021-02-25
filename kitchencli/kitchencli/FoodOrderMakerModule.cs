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
    /// Handles receiving an Order from the IOrderReceiver
    /// The orders will added to an OrderTicket object that has a timer delegate
    /// When the timer expires, the delegate is called and passes the order to the
    /// IFoodOrderStatusManager, where it will be handed to a Courier
    /// </summary>
    internal class FoodOrderMakerModule : IFoodOrderMaker
    {
        
        public FoodOrderMakerModule(ICourierOrderMatcher foodMatcher)
        {

        }

        public void PrepareOrder(Order order)
        {
            
        }
    }
}
