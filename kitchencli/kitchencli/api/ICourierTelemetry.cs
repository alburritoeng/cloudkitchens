using System;

namespace kitchencli.api
{
    public interface ICourierTelemetry
    {
        DateTime ArrivalTime { get; set; }
        
        DateTime OrderPickupTime { get; set; }
    }
}