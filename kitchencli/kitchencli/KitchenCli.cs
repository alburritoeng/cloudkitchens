using kitchencli.api;
using kitchencli.CourierMatch;
using kitchencli.utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli
{
    /// <summary>
    /// this is the main entry for this project, sets up all the objects
    /// also has-a producer for creating and putting orders into the system
    /// </summary>
    class KitchenCli : IKitchen
    {

        internal IOrderReceiver orderReceiver;

        internal IFoodOrderMaker foodOrderMaker;

        internal ICourierFactory courierFactory;

        internal ICourierOrderMatcher courierOrderMatcher;

        string _jsonFile;

        DispatchCourierMatchEnum _dispatcherCourierType;

        List<Order> orders = null;
                
        CreatedOrderConsumer _orderConsumer;
        public KitchenCli()
        {
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

            courierFactory = new RandomCourierFactory();
            switch (dipatcherType)
            {
                case DispatchCourierMatchEnum.F:
                    courierOrderMatcher = new CourierOrderFifo();
                    break;
                default:
                    courierOrderMatcher = new CourierOrderMatch();
                    break;
            }
            
            foodOrderMaker = new FoodOrderMakerModule(courierOrderMatcher);
            orderReceiver = new OrderReceiverModule(foodOrderMaker, courierFactory);
            _orderConsumer = new CreatedOrderConsumer(orderReceiver);            
        }

        public void Start()
        {
            foreach (Order order in orders)
            {
                _orderConsumer.AddOrderToQueue(order);
            }
        }

        public void Stop()
        {
            _orderConsumer.Stop();
        }
    }
}
