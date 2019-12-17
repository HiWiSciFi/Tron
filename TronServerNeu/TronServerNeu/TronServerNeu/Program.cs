using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TronServerNeu
{
    class Program
    {
        private static  IPEndPoint ipEp;
        private static  byte version;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// erstes Argument listenport:<port>
        /// zweites Argument gameversion:<version>
        /// </param>
        static void Main(string[] args)
        {
            ProcessArgs(args);

            
            
        }


        /// <summary>
        /// Einlesen der Argumente ipEp und version werden gesetzt
        /// </summary>
        /// <param name="args">
        /// erstes Argument listenport:<port>
        /// zweites Argument gameversion:<version>
        /// </param>
        private static void ProcessArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().StartsWith("listenport:"))
                {
                    try
                    {
                        string[] split = args[i].Split(':');

                        ipEp = new IPEndPoint(IPAddress.Any,int.Parse(split[1]));
                        
                        continue;
                    }
                    catch (Exception e){
                        Console.WriteLine(e);
                    }
                }

                if (args[i].ToLower().StartsWith("gameversion:"))
                {
                    try
                    {
                        string[] split = args[i].Split(':');
                        version = byte.Parse(split[1]);
                        continue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }
    }
}
