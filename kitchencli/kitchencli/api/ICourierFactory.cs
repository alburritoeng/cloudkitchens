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
    public interface ICourierFactory
    {
        ICourier CreateCourier(Order order);

        void ReturnCourier(ICourier courier);
    }
}
