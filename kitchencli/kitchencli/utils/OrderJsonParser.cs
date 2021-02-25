using Newtonsoft.Json;
using System;

namespace kitchencli.utils
{
    public static class OrderJsonParser
    {
        public static Order CreateOrderFromJSON(string jsonString)
        {
            if(string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            Order order;
            try
            {
                order = JsonConvert.DeserializeObject<Order>(jsonString);
            }
            catch(Exception )
            {
                return null;
            }

            return order;
        }
    }
}
