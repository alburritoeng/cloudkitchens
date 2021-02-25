using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.utils
{
    /// <summary>
    /// A concrete class to describe the JSON we will parse to create orders
    /// Input is in JSON, internally we will use C# objects to describe the JSON.
    /// Those C# objects will be the way we pass orders around internally through the workflow    
    /// </summary>
    public class Order
    {
        public Order()
        {
            id = Guid.NewGuid().ToString();
            prepTime = -1;
            name = string.Empty;
            CreatedTime = DateTime.Now;
        }
        /// <summary>
        /// The id of the Order
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The name of the Order
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The time for PrepTime (in seconds) for Order
        /// </summary>
        public int prepTime { get; set; }

        public DateTime CreatedTime { get; private set; }
    }   
    
}
