using kitchencli.utils;

namespace kitchencli.api
{
    public enum MatchType
    {
        Match,
        Fifo
    }
    /// <summary>
    /// This interface receives Orders and handles logic for 
    /// Couriers picking up orders
    /// 
    /// Concrete implementations of this interface can implement Match or First-in-First-Out logic for 
    /// order hand off to Couriers
    /// </summary>
    public interface ICourierOrderMatcher
    {
        /// <summary>
        /// To be called once an order is finished, this sets the order ready for pickup.
        /// This should match an order with a courier, if possible
        /// </summary>
        /// <param name="order"></param>
        void AddToOrderReadyQueue(Order order);

        /// <summary>
        /// To be called by when a courier has arrived, this allows a courier to be matched with a ready order, if possible
        /// </summary>
        /// <param name="courier"></param>
        void CourierArrived(ICourier courier);

        /// <summary>
        /// Returns one of two types: Match or Fifo
        /// </summary>
        /// <returns>MatchType enum with either Match or Fifo, set from Cli</returns>
        MatchType GetMatchType();
    }
}
