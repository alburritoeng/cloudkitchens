using kitchencli.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kitchencli
{
    class Program
    {
        /// <summary>
        /// This program will serve as the Cli (cli/json/order-originator)
        /// There is an argparser for handling user input
        /// JsonOrderInputFile -f <filePath>
        /// CourierType -c <MorF>  (Match or First in First Out)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // parse the arguments
            KitchenCli cli = null;
            IDictionary<string, string> arguments = new Dictionary<string, string>();
            ArgResultEnum result = ArgumentParser.ParseAguments(args, ref arguments);
            if(result == ArgResultEnum.Error)
            {
                Console.WriteLine($"{ArgumentParser.HelperString}");
            }
            else
            {
                foreach(var a in arguments)
                {
                    Console.WriteLine($"{a}");
                }
                Task.Run(() =>
                {
                    cli = new KitchenCli();
                    cli.Initialize(arguments["f"], (DispatchCourierMatchEnum)Enum.Parse(typeof(DispatchCourierMatchEnum), arguments["c"], true));
                    cli.Start();
                });
            }

            Console.Read();
            cli?.Stop();
        }
    }
}
