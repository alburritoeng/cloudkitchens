using System;

namespace kitchencli.api
{
    /// <summary>
    /// interface for adding telemetry to a courier object
    /// </summary>
    public interface ICourierTelemetry
    {
        /// <summary>
        /// The time the courier arrived to pick up an order
        /// </summary>
        DateTime ArrivalTime { get; set; }
        
        /// <summary>
        /// the time the courier picked up an order
        /// </summary>
        DateTime OrderPickupTime { get; set; }
    }
}