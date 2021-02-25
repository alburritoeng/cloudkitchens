using kitchencli.utils;
using System;

namespace kitchencli.api
{
    /// <summary>
    /// A Courier will be created by the CourierFactory
    /// It will have a property for order Id
    /// It will have a property for created time    
    /// </summary>
    public interface ICourier: ICourierTelemetry
    {
        Action<ICourier> NotifyArrivedForOrder { get; set; }

        Order CurrentOrder { get; set; }
        /// <summary>
        /// returns the type of courier, for example ubereats, doordash, uncle-pete's fast food deliver svc
        /// </summary>
        /// <returns></returns>
        string CourierType();

        /// <summary>
        /// returns int, should be an enum...if theres time, of courier
        /// </summary>
        /// <returns></returns>
        int CourierTypeInt();

        /// <summary>
        /// The IOrderReadyPass object will assign the order and set this Id. 
        /// The Order assigned is by Match or FIFO, or otherwise (future)
        /// The Id will be used to pick up an Order. 
        /// </summary>
        Guid OrderId();

        /// <summary>
        /// The Time for arrival, will be a random value between 3-15seconds, after which this Courier
        /// will "arrive" for pickup
        /// </summary>
        int DurationEstimateInSeconds();

        /// <summary>
        /// This method is intended to start the internal timer that will run for DurationEstimate
        /// and once that elapses, it will call the FoodOrdeMaker to indicate "I've arrived for the order"
        /// </summary>
        void LeaveForFood();

        /// <summary>
        /// an Id to uniquely identify this Courier
        /// </summary>
        Guid CourierUniqueId { get; }
    }
}
