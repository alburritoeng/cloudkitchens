using kitchencli.api;
using kitchencli.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli
{
    class CreatedOrderConsumer
    {
        private Queue<Order> _ordersQueue;
        private readonly object _lock = new object();
        private CancellationTokenSource cts;
        private IOrderReceiver _orderReceiver;        
        
        public CreatedOrderConsumer(IOrderReceiver orderReceiver)
        {
            _orderReceiver = orderReceiver;
            cts = new CancellationTokenSource();
            _ordersQueue = new Queue<Order>();
            Task.Run(() =>
            {
                ProcessOrders(cts.Token);
            });
        }
        
        public void AddOrderToQueue(Order order) 
        {
            // could use thread safe queue (out-of-the-box), but 
            // going to use a lock instead :)
            lock(_lock)
            {
                _ordersQueue.Enqueue(order);
            }
        }

        private Order PickOrder()
        {
            lock(_lock)
            {
                if(_ordersQueue.Count > 0 && _ordersQueue.Peek()!=null)
                {
                    return _ordersQueue.Dequeue();
                }
            }
            return null;
        }

        public void Stop()
        {
            cts.Cancel();
        }

        private void ProcessOrders(CancellationToken token)
        {
            do
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Order taking process has been stopped");
                    break;
                }

                // pick 2 orders/second
                Order order = SendSingleOrder();                
                Thread.Sleep(1000);

            } while (true);            
        }

        private Order SendSingleOrder()
        {
            var order = PickOrder();
            if (order != null)
            {
                Console.WriteLine($"Sending Order {order.id}\t{order.name} to Kitchen");
                _orderReceiver.SendOrderToOrderMaker(order);
                Console.WriteLine($"Dispatching Courier for Order {order.id}\t{order.name}");
                _orderReceiver.DispatchCourier(order);
            }

            return order;
        }
    }
}
