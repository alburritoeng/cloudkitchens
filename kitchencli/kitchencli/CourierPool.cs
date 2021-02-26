using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using System;
using System.Collections.Concurrent;

namespace kitchencli
{
    internal class CourierPool : IStartStoppableModule, ICourierPool
    {
        private int maxCouriers = 15;
        internal readonly ConcurrentQueue<ICourier> _courierPoolSet;

        public int MaxCouriers => maxCouriers;

        private void InitializePool()
        {
            for (int i = 0; i < maxCouriers; i++)
            {
                _courierPoolSet.Enqueue(GetRandomCourier());
            }
        }

        private ICourier GetRandomCourier()
        {
            int type = random.Next(1, 4);
            switch (type)
            {
                case 1:
                    return new UberEatsCourier();
                case 2:
                    return new DoorDashCourier();
                default:
                    return new GrubHubCourier();
            }
        }
        
        private Random random;
        public CourierPool()
        {
            int seed = DateTime.Now.Second;
            random = new Random(seed);
            _courierPoolSet = new ConcurrentQueue<ICourier>();
        }

        public ICourier GetCourier()
        {
            // attempt to give from pool
            // if pool empty, create new one
            if (!_courierPoolSet.IsEmpty && (_courierPoolSet.TryDequeue(out var courier)))
            {
                return courier;
            }
            
            return GetRandomCourier();
        }

        public void ReturnCourier(ICourier courier)
        {
            courier.RecalcDuration();
            _courierPoolSet.Enqueue(courier);
        }

        public void Start()
        {
            InitializePool(); 
        }

        public void Stop()
        {
            // I know, GCC, but trying to unpin these objects, to be a good little dev
            bool res = false;
            do
            {
                res = _courierPoolSet.TryDequeue(out _);
            } while (res != false);
        }
    }
}
