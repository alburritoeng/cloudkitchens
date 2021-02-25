using kitchencli.utils;

namespace kitchencli.api
{
    /// <summary>
    /// The purpose of this interface is to receive an Order file
    /// from the cli/json/order-originator
    /// </summary>
    interface IOrderReceiver
    {
        /// <summary>
        /// Uses the orderId to ask factory for a Courier
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns>a Courier</returns>
        void DispatchCourier(Order order);

        /// <summary>
        /// sends order to the order maker for making
        /// </summary>
        /// <param name="order"></param>
        void SendOrderToOrderMaker(Order order);
    }
}
