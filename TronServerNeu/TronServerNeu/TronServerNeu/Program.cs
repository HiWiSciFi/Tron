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
        private static TcpListener listener;
        private static FreeIDs freeIDs;

        /// <summary>
        /// all conected players
        /// </summary>
        private static List<Player> players;
        /// <summary>
        /// all players in a Loby 
        /// in the future a Loby class is planed right now Programm is THE loby
        /// </summary>
        private static List<Player> inLobyPlayers;
        /// <summary>
        /// conected players outside a loby 
        /// </summary>
        private static List<Player> pendingPlayers;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// first argument listenport:<port>
        /// secund argument gameversion:<version>
        /// </param>
        static void Main(string[] args)
        {
            ProcessArgs(args);

            PreStart();

            StartServer();
            
        }

        
        /// <summary>
        /// Reading the arguments and seting version and ipEp
        /// </summary>
        /// <param name="args">
        /// first argument listenport:<port>
        /// secund argument gameversion:<version>
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

        /// <summary>
        /// Starting TcpListener on ipEp
        /// </summary>
        private static void StartServer()
        {
            
            
            listener = TcpListener.Create(ipEp.Port);
            Console.WriteLine("Starting listener on port " + ipEp.Port + " using IP " + ipEp.Address);

            listener.Start();

            Console.WriteLine("Server running on port " + ipEp);

            //legac vielleicht gut: connectedPlayers = new List<Player>();

            Console.WriteLine("Server started");
            Console.WriteLine("----------------------------------");
        }

        /// <summary>
        /// initialises important variables:
        ///     freeIDs
        ///     players
        ///     inLobyPlayers
        ///     pendingPlayers
        /// </summary>
        private static void PreStart()
        {
            freeIDs = new FreeIDs();
            players = new List<Player>();
            inLobyPlayers = new List<Player>();
            pendingPlayers = new List<Player>();

            Console.WriteLine("prestart procedings have been dealt with");
        }
    }
}
