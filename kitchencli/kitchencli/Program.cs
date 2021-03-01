using kitchencli.api;
using System;
using System.Collections.Generic;
using System.Threading;
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
            Bootstrapper bootstrapper = null;
            ManualResetEvent evt = new ManualResetEvent(false);
            IDictionary<string, string> arguments = new Dictionary<string, string>();
            ArgResultEnum result = ArgumentParser.ParseAguments(args, ref arguments);
            if(result == ArgResultEnum.Error)
            {
                Console.WriteLine($"{ArgumentParser.HelperString}");
                return;
            }

            foreach(KeyValuePair<string, string> a in arguments)
            {
                Console.WriteLine($"{a}");
            }
            DateTime startTime = DateTime.Now;
            
            Task.Run(() =>
            {
                bootstrapper = new Bootstrapper(evt);
                bootstrapper.Initialize(arguments["f"], (DispatchCourierMatchEnum)Enum.Parse(typeof(DispatchCourierMatchEnum), arguments["c"], true));
                bootstrapper.Start();
            });

            // wait for the stop event indefinitely
            evt.WaitOne();
            
            evt?.Dispose();
            Console.WriteLine(($"Total Time: {DateTime.Now.Subtract(startTime).TotalMinutes} minutes"));
            Console.WriteLine("Exit");
        }
    }
}
