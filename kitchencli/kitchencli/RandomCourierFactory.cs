using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli
{
    internal class RandomCourierFactory : ICourierFactory
    {
        Dictionary<int, IList<ICourier>> courierQueue;
        private readonly object _lock = new object();
        private Random random;
        public RandomCourierFactory()
        {
            int seed = DateTime.Now.Second;
            random = new Random(seed);
            courierQueue = new Dictionary<int, IList<ICourier>>();
        }

        public ICourier CreateCourier(Order order)
        {
            switch (random.Next(1, 3))
            {
                case 1:                    
                    return new UberEatsCourier(order);
                case 2:
                    return new DoorDashCourier(order);
                default:
                    return new GrubHubCourier(order);
            }
        }

        public void ReturnCourier(ICourier courier)
        {
            lock(_lock)
            {
            }
        }
    }
}
