using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.api
{
    /// <summary>
    /// A Courier will be created by the CourierFactory
    /// It will have a property for order Id
    /// It will have a property for created time    
    /// </summary>
    public interface ICourier
    {
        
        /// <summary>
        /// returns the type of courier, for example ubereats, doordash, uncle-pete's fast food deliver svc
        /// </summary>
        /// <returns></returns>
        string CourierType();

        /// <summary>
        /// Describes when this courier was created, for telemetry purposes - perhaps 
        /// we can use this to tell us how long a courier has been in service. The current idea
        /// is to re-use couriers after they have made their delivery
        /// </summary>
        DateTime CreatedTime();

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
        /// the order handed to the courier during pickup
        /// </summary>
        Order OrderToDeliver();
    }
}
