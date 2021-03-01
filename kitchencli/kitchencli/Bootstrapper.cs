using kitchencli.api;
using kitchencli.CourierMatch;
using kitchencli.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace kitchencli
{
    /// <summary>
    /// this is the main entry for this project, sets up all the objects
    /// also has-a producer for creating and putting orders into the system
    /// </summary>
    class Bootstrapper : IKitchenCli
    {
        internal readonly IList<object> _modules;
        
        internal IOrderReceiver _orderReceiver;

        internal IKitchen kitchen;

        internal ICourierPool CourierPool;

        internal ICourierOrderMatcher courierOrderMatcher;

        string _jsonFile;

        DispatchCourierMatchEnum _dispatcherCourierType;

        List<Order> orders = null;
                
        IStartStoppableModule _orderConsumer;

        private int _totalOrders;
        private object _lock = new object();
        private ManualResetEvent _evt;
        
        private const int ExitDelayMs = 250;
        public Bootstrapper(ManualResetEvent evt)
        {
            _evt = evt;
            _modules = new List<object>();
            orders = null;
            _jsonFile = string.Empty;
            _dispatcherCourierType = DispatchCourierMatchEnum.Unknown;            
        }               

        public void Initialize(string jsonFile, DispatchCourierMatchEnum dipatcherType)
        {
            _jsonFile = jsonFile;
            _dispatcherCourierType = dipatcherType;
          
            using (StreamReader r = new StreamReader(_jsonFile))
            {
                string json = r.ReadToEnd();
                try
                {
                    orders = JsonConvert.DeserializeObject<List<Order>>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception loading order file {e}");
                    return;
                }
            }

            if (orders == null)
            {
                Console.WriteLine($"There were no orders found in the order file");
                return;
            }

            _totalOrders = orders.Count;
            OrderDeliveredPublisher.Subscribe(OrderedDelivered);
            CourierPool = new CourierPool();
            switch (dipatcherType)
            {
                case DispatchCourierMatchEnum.F:
                    courierOrderMatcher = new CourierOrderFifo(CourierPool, new TelemetryModule());
                    break;
                default:
                    courierOrderMatcher = new CourierOrderMatch(CourierPool, new TelemetryModule());
                    break;
            }
            
            kitchen = new KitchenModule(courierOrderMatcher);
            _orderReceiver = new OrderReceiverModule(kitchen, CourierPool, courierOrderMatcher);
            _orderConsumer = new CreatedOrderConsumer(_orderReceiver);
           
            _modules.Add(_orderReceiver);
            _modules.Add(kitchen);
            _modules.Add(CourierPool);
            _modules.Add(courierOrderMatcher);
            _modules.Add(_orderConsumer);
        }

        private async void OrderedDelivered(string orderId)
        {
            bool exit = false;
            lock (_lock)
            {
                _totalOrders--;
                if (_totalOrders == 0)
                {
                    exit = true;
                }
            }

            if (!exit)
            {
                return;
            }
            
            // waiting some time to give things time a chance to print to the console screen. 
            await Task.Delay(ExitDelayMs);
            Console.WriteLine($"All ordered delivered!");
            Stop();
        }
        public void Start()
        {
            foreach (object module in _modules)
            {
                if (module is IStartStoppableModule)
                {
                    ((IStartStoppableModule)module).Start();
                }
            }
            
            // consume the json file
            foreach (Order order in orders)
            {
                ((CreatedOrderConsumer)_orderConsumer).AddOrderToQueue(order);
            }
        }

        public void Stop()
        {
            foreach (object module in _modules)
            {
                if (module is IStartStoppableModule)
                {
                    ((IStartStoppableModule)module).Stop();
                }

                if (module is IDisposable)
                {
                    ((IDisposable)module).Dispose();
                }
            }

            _evt.Set();
        }
    }
}
