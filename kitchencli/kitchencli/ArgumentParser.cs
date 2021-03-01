using kitchencli.api;
using System;
using System.Collections.Generic;
using System.IO;

namespace kitchencli
{
    public enum ArgResultEnum
    {
       Error = 0,
       Success
    }

    public static class ArgumentParser
    {
        
        public static string HelperString => "Usage: \n-f <filepath> [required] \n-c <M for Match or F First-in-First-Out> [required]";

        public static ArgResultEnum ParseAguments(string[] args, ref IDictionary<string, string> arguments)
        {
            if (args.Length <= 2)
            {
                return ArgResultEnum.Error;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                {
                    continue;
                }

                // check we are starting w/ the the correct argument indicator '-'
                if (args[i].StartsWith("-"))
                {
                    string flag = args[i].Substring(1);
                    if (string.IsNullOrEmpty(flag))
                    {
                        Console.WriteLine($"Invalid argument: {args[i]}");
                        return ArgResultEnum.Error;
                    }

                    // check that the flag is one of the ones we support
                    switch (flag)
                    {
                        case "f":
                        case "F":
                            if(!AddArgumentToList(args, flag, arguments, i, (argmentReceived) =>
                            {
                                bool res= File.Exists(argmentReceived);
                                if(!res)
                                {
                                    Console.WriteLine($"File at path '{argmentReceived}' does not exist.");
                                }
                                return res;
                            }))
                            {
                                Console.WriteLine($"Invalid argument: {args[i]}");
                                return ArgResultEnum.Error;
                            }
                            break;
                        case "c":
                        case "C":
                            if (!AddArgumentToList(args, flag, arguments, i, (argmentReceived) => 
                            {
                                return Enum.TryParse<DispatchCourierMatchEnum>(argmentReceived, out _);                                
                            }))
                            {
                                Console.WriteLine($"Invalid argument: {args[i]}");
                                return ArgResultEnum.Error;
                            }
                            break;
                        default:
                            Console.WriteLine($"Invalid argument: {args[i]}");
                            return ArgResultEnum.Error;

                    }
                }               
            }

            if(arguments.Count == 0)
            {
                return ArgResultEnum.Error;
            }

            return ArgResultEnum.Success;
        }

        private static bool AddArgumentToList(string[] args, string flag, IDictionary<string, string> arguments, int i, Func<string,bool> validate=null)
        {
            if (args[i + 1] != null)
            {
                if (validate != null)
                {
                    if(validate(args[i + 1]) == false)
                    {
                        return false;
                    }
                }
                arguments.Add(flag.ToLower(), args[i + 1].ToLower());
                return true;
            }
            return false;
        }
    }
}
