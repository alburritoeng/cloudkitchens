
namespace kitchencli.api
{
    public interface ICourierOrderTelemetry
    {
        void CalculateAverageFoodWaitTime(IOrderTelemetry orderTelemetry);
        
        void CalculateAverageCourierWaitTime(ICourierTelemetry courierTelemetry);
    }
}