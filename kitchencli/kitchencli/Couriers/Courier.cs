using kitchencli.api;
using kitchencli.utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli.Couriers
{
    abstract class Courier : ICourier
    {
        internal int _durationEstimate;
        //protected Order _currentOrder;
        public Courier()
        {
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

        public virtual string CourierType()
        {
            throw new NotImplementedException();
        }

        public virtual int CourierTypeInt()
        {
            return 0;
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
