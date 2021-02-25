using kitchencli.api;
using kitchencli.Couriers;
using kitchencli.utils;
using System;
using System.Collections.Generic;

namespace kitchencli
{
    internal class RandomCourierFactory : ICourierFactory
    {
        readonly Dictionary<int, Queue<ICourier>> _courierQueue;
        private readonly object _lock = new object();
        private Random random;
        public RandomCourierFactory()
        {
            int seed = DateTime.Now.Second;
            random = new Random(seed);
            _courierQueue = new Dictionary<int, Queue<ICourier>>();
            _courierQueue[1] = new Queue<ICourier>();
            _courierQueue[2] = new Queue<ICourier>();
            _courierQueue[3] = new Queue<ICourier>();
        }

        public ICourier CreateCourier(Order order)
        {
            ICourier courier = null;
            int type = random.Next(1, 3);
            switch (type)
            {
                case 1:
                    //courier = GetFromCache(type, order);
                    if (courier == null)
                    {
                        courier = new UberEatsCourier(order);
                    }
                    break;
                case 2:
                    //courier = GetFromCache(type, order);
                    if (courier == null)
                    {
                        courier = new DoorDashCourier(order);
                    }
                    break;
                default:
                    //courier = GetFromCache(type, order);
                    if (courier == null)
                    {
                        courier = new GrubHubCourier(order);
                    }
                    break;
            }
            return courier;
        }

        private ICourier GetFromCache(int type, Order order)
        {
            lock (_lock)
            {
                if (_courierQueue.ContainsKey(type) && _courierQueue[type].Count>0)
                {
                    ICourier courier= _courierQueue[type].Dequeue();
                    courier.CurrentOrder = order;
                    Console.WriteLine($"From Cache: Total Couriers of type {courier.CourierType()} = {_courierQueue[courier.CourierTypeInt()].Count}");
                    return courier;
                }
            }

            return null;
        }
        public void ReturnCourier(ICourier courier)
        {
            lock(_lock)
            {
                
            }
        }
    }
}
