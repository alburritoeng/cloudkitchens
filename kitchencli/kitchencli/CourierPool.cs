using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using System;
using System.Collections.Concurrent;

namespace kitchencli
{
    /// <summary>
    /// This class is intended to server as a pool of Couriers that are created up front during initialization time.
    /// The goal is to lessen the burden of creating new couriers during execution of the routine.
    ///
    /// There are 3 concrete types of couriers created, DoorDash, GrubHub, and UberEats. The goal is to randomly
    /// generate 15 couriers, consisting of hopefully a random amount of each type of courier. The number 15 of
    /// maxCouriers was selected as I ran this routine over and over with the input file of 132 orders. I found that
    /// I never depleted the number of couriers to zero. I some times got to a value of 1 available courier in the
    /// pool. I also considered that perhaps the team testing this code would probably use a file of 250 or 500 or 1000
    /// orders, so I allowed the ability to generate new Couriers on the fly. 
    /// </summary>
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
            int type = random.Next(0, 3); 
            return new Courier(type);            
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
            ICourier courier;
            // attempt to give from pool
            // if pool empty, create new one
            if (!_courierPoolSet.IsEmpty && (_courierPoolSet.TryDequeue(out courier)))
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierPool] Courier {courier.CourierUniqueId} dispatched");
                return courier;
            }
            
            courier = GetRandomCourier();
            Console.WriteLine($"{DateTime.Now.TimeOfDay} [CourierPool] Courier {courier.CourierUniqueId} dispatched");
            return courier;
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
            // I wanted to unpin these objects from the GCC because pinning prevents the objects
            // to be moved by the garbage collector. I realize we are stopping, but we can optimize this
            // application to be stoppable, but not exit. In that case, we'd want to have our memory released. 
            bool res = false;
            do
            {
                res = _courierPoolSet.TryDequeue(out _);
            } while (res != false);
        }
    }
}
