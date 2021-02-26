
namespace kitchencli.api
{
    /// <summary>
    /// Telemetry for managing time of food and courier
    /// </summary>
    public interface ICourierOrderTelemetry
    {
        /// <summary>
        /// Called to keep running average of Food wait/pick up times
        /// </summary>
        /// <param name="orderTelemetry"></param>
        void CalculateAverageFoodWaitTime(IOrderTelemetry orderTelemetry);
        
        /// <summary>
        /// Called to keep running average of Courier wait/pick up times
        /// </summary>
        /// <param name="courierTelemetry"></param>
        void CalculateAverageCourierWaitTime(ICourierTelemetry courierTelemetry);
    }
}