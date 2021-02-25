using System;

namespace kitchencli.api
{
    public interface IOrderTelemetry
    {
        /// <summary>
        /// To Be set when the order is deemed ready for pick up
        /// </summary>
        DateTime OrderReadyTime { get; set; }
        
        /// <summary>
        /// to be set when the order is actually picked up by a courier
        /// </summary>
        DateTime OrderPickUpTime { get; set; }
    }
}