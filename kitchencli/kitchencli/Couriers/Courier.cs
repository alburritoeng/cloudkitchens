using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.Couriers
{
    abstract class Courier : ICourier
    {
        protected int _durationEstimate = -1;
        protected Order _currentOrder;
        public Courier()
        {
            _durationEstimate = RandomDistributionGenerator.GetRandomDistribution();
        }

        public Order CurrentOrder
        {
            get
            {
                return _currentOrder;
            }
            private set
            {
                _currentOrder = value;
            }
        }

        public virtual string CourierType()
        {
            throw new NotImplementedException();
        }

        public virtual DateTime CreatedTime()
        {
            if (CurrentOrder == null)
            {
                return DateTime.MinValue;
            }

            return CurrentOrder.CreatedTime;
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

        public Order OrderToDeliver()
        {
            return CurrentOrder;
        }
    }
}
