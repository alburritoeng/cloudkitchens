using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli.utils
{
    public static class RandomDistributionGenerator
    {
        static int seed = DateTime.Now.Millisecond;
        static Random random = new Random(seed);
        public static int GetRandomDistribution()
        {
            return random.Next(3, 15);
        }
    }
}
