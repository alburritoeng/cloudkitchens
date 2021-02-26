using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kitchencli.api;

namespace kitchencli.utils
{
    internal class TelemetryModule : ICourierOrderTelemetry
    {
        
        private List<TimeSpan> FoodReadyToPickupTimes;
        private List<TimeSpan> CourierArrivalToPickupTime;

        private readonly object _foodLock = new object();
        private readonly object _courierLock = new object();
        public TelemetryModule()
        {
            FoodReadyToPickupTimes = new List<TimeSpan>();
            CourierArrivalToPickupTime = new List<TimeSpan>();
        }

        private TimeSpan GetTimeSpanAverage(List<TimeSpan> list)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.average?view=netframework-4.7.2
            double dAverageTicks = list.Average(ts => ts.Ticks);
            long lAverageTickets = Convert.ToInt64(dAverageTicks);
            return new TimeSpan(lAverageTickets);
        }
        
        public void CalculateAverageFoodWaitTime(IOrderTelemetry orderTelemetry)
        {
            Task.Run(() =>
            {
                lock (_foodLock)
                {
                    TimeSpan ts = orderTelemetry.OrderPickUpTime.Subtract(orderTelemetry.OrderReadyTime);
                    FoodReadyToPickupTimes.Add(ts);
                    var t = GetTimeSpanAverage(FoodReadyToPickupTimes);
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [TELEMETRY - FOOD] Average Order Ready to Pickup Time {t.Milliseconds} ms");
                }
            });
        }
        
        public void CalculateAverageCourierWaitTime(ICourierTelemetry courierTelemetry)
        {
            Task.Run(() => 
            { 
                lock (_courierLock)
                {
                    TimeSpan ts = courierTelemetry.OrderPickupTime.Subtract(courierTelemetry.ArrivalTime);
                    CourierArrivalToPickupTime.Add(ts);
                    var t = GetTimeSpanAverage(CourierArrivalToPickupTime);
                    Console.WriteLine($"{DateTime.Now.TimeOfDay} [TELEMETRY - COURIER] Average Arrival to Pickup Time {t.Milliseconds} ms");
                }
            });
        }
    }
}
