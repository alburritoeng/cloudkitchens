using kitchencli.api;
using kitchencli.utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli.Couriers
{
    public class Courier : ICourier
    {
        internal int _durationEstimate;
        private int _courierType = -1;
        
        public Courier()
        {
            _durationEstimate = RandomDistributionGenerator.GetRandomDistribution();
            CourierUniqueId = Guid.NewGuid();
            _courierType = 0;// arbitrary decided to default to type 0 for couriers
        }
        
        public Courier(int courierType) 
        {
            _courierType = courierType;
            _durationEstimate = RandomDistributionGenerator.GetRandomDistribution();
            CourierUniqueId = Guid.NewGuid();
        }

        private Order _c;
        public Order CurrentOrder
        {
            get { return _c;}
            set
            {
                _c = value;
            }
        }
        
        public Action<ICourier> NotifyArrivedForOrder { get; set; }

        public string CourierType()
        {
            switch (_courierType)
            {
                case 0:
                    return "UberEats";
                case 1:
                    return "DoorDash";
                default:
                    return "GrubHub";
            }
        }

        public virtual int CourierTypeInt()
        {
            return _courierType;
        }

        public virtual int DurationEstimateInSeconds()
        {
            return _durationEstimate;
        }

        public Guid OrderId()
        {
            if (CurrentOrder == null)
            {
                return Guid.Empty;
            }

            if (Guid.TryParse(CurrentOrder.id, out Guid guid))
            {
                return guid;
            }
            return Guid.Empty;
        }

        public void LeaveForFood()
        {
            Task.Run(() =>
            {
                using (ManualResetEvent evt = new ManualResetEvent(false))
                {
                    evt.WaitOne(_durationEstimate * 1000);

                    NotifyArrivedForOrder?.Invoke(this);
                }
            });
        }

        public Guid CourierUniqueId { get; set; }
        public void RecalcDuration()
        {
            _durationEstimate = RandomDistributionGenerator.GetRandomDistribution();
        }

        public DateTime ArrivalTime { get; set; }
        public DateTime OrderPickupTime { get; set; }
    }
}
