using kitchencli.api;
using kitchencli.CourierMatch;
using kitchencli.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace kitchencli
{
    /// <summary>
    /// this is the main entry for this project, sets up all the objects
    /// also has-a producer for creating and putting orders into the system
    /// </summary>
    class Bootstrapper : IKitchen
    {
        internal readonly IList<object> _startStoppableObjects;
        
        internal IOrderReceiver _orderReceiver;

        internal IFoodOrderMaker _foodOrderMaker;

        internal ICourierFactory _courierFactory;

        internal ICourierOrderMatcher courierOrderMatcher;

        string _jsonFile;

        DispatchCourierMatchEnum _dispatcherCourierType;

        List<Order> orders = null;
                
        IStartStoppableModule _orderConsumer;
        public Bootstrapper()
        {
            _startStoppableObjects = new List<object>();
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

            _courierFactory = new RandomCourierFactory();
            switch (dipatcherType)
            {
                case DispatchCourierMatchEnum.F:
                    courierOrderMatcher = new CourierOrderFifo(_courierFactory, new TelemetryModule());
                    break;
                default:
                    courierOrderMatcher = new CourierOrderMatch(_courierFactory, new TelemetryModule());
                    break;
            }
            
            _foodOrderMaker = new FoodOrderMakerModule(courierOrderMatcher);
            _orderReceiver = new OrderReceiverModule(_foodOrderMaker, _courierFactory, courierOrderMatcher);
            _orderConsumer = new CreatedOrderConsumer(_orderReceiver);
            
            if (_foodOrderMaker is IStartStoppableModule)
            {
                _startStoppableObjects.Add(_foodOrderMaker);
            }
            if (_orderReceiver is IStartStoppableModule)
            {
                _startStoppableObjects.Add(_orderReceiver);
            }
            
            _startStoppableObjects.Add(_orderConsumer);
        }

        public void Start()
        {
            foreach (IStartStoppableModule startableObject in _startStoppableObjects)
            {
                startableObject.Start();
            }
            
            // consume the json file
            foreach (Order order in orders)
            {
                ((CreatedOrderConsumer)_orderConsumer).AddOrderToQueue(order);
            }
        }

        public void Stop()
        {
            _orderConsumer.Stop();
            foreach (IStartStoppableModule stoppableObject in _startStoppableObjects)
            {
                stoppableObject.Stop();
            }
        }
    }
}
