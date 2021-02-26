using kitchencli.utils;

namespace kitchencli.api
{
    /// <summary>
    /// The purpose of this api is to receive an Order, 
    /// add the order to the order queue and send complete orders to "the pass" (ICourierOrderMatcher)
    /// pickup by Couriers
    /// 
    /// IWO this interface is to manage the order's life cycle after it is received from the cli/json/order-originator
    /// </summary>
    public interface IKitchen
    {
        /// <summary>
        /// Kitchen receives an order and uses preptime as indicator for how long it takes to prepare an order
        /// after which, order is deemed ready for pickup
        /// </summary>
        /// <param name="order"></param>
        void PrepareOrder(Order order);

        /// <summary>
        /// The Kitchen (establishment preparing the food) has a courier arrive and pick up an order
        /// </summary>
        /// <param name="courier"></param>
        void CourierHasArrived(ICourier courier);
    }
}
