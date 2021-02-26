using kitchencli.utils;

namespace kitchencli.api
{
    /// <summary>
    /// This interface is intended to be a Courier Factory Method 
    /// It exposes API for generating a Courier
    /// we can have different types of couriers: doordash, grubhub, uber eats, etc
    /// 
    /// Implementations would create an Object of type ICourier
    /// </summary>
    public interface ICourierPool
    {
        /// <summary>
        /// returns an instance of ICourier
        /// </summary>
        /// <returns></returns>
        ICourier GetCourier();

        /// <summary>
        /// an instance of ICourier is returned to the Courier Object Pool
        /// </summary>
        /// <param name="courier"></param>
        void ReturnCourier(ICourier courier);
    }
}
