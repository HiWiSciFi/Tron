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
            if (!ProcessArgs(args))
            {
                Console.WriteLine("arguments missing");
                return;
            }

            PreStart();
            StartServer();

            Loop();
            
        }

        /// <summary>
        /// Reading the arguments and seting version and ipEp
        /// </summary>
        /// <param name="args">
        /// first argument listenport:<port>
        /// secund argument gameversion:<version>
        /// </param
        /// <returns>whether version and ipEp was found</returns>
        private static bool ProcessArgs(string[] args)
        {
            bool portFound = false;
            bool versionFound = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().StartsWith("listenport:"))
                {
                    

                    try
                    {
                        string[] split = args[i].Split(':');

                        ipEp = new IPEndPoint(IPAddress.Any,int.Parse(split[1]));
                        portFound = true;
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
                        versionFound = true;
                        continue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            return portFound && versionFound;
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

        /// <summary>
        /// Starting TcpListener on ipEp
        /// </summary>
        private static void StartServer()
        {
            
            
            listener = TcpListener.Create(ipEp.Port);
            Console.WriteLine("Starting listener on port " + ipEp.Port + " using IP " + ipEp.Address);

            listener.Start();

            Console.WriteLine("Server running on port " + ipEp);

            //legac vielleicht gut: players = new List<Player>();

            Console.WriteLine("Server started");
            Console.WriteLine("----------------------------------");
        }

        
        private static void Loop()
        {
            try
            {

                while (true)
                {
                    HandleConections();


                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void HandleConections()
        {

            HandleDisconects();
            AkwardHandshaking();
        }


        private static void HandleDisconects()
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (!SocketConnected(players[i].socket))
                {
                    Console.WriteLine();
                    Console.WriteLine("Player " + players[i].ID + " Disconnected");
                    Console.WriteLine("Informing Clients");
                    for (int j = 0; j < players.Count; j++)
                    {
                        if (j != i)
                        {
                            //notify players 
                            throw new NotImplementedException();
                        }
                    }
                    Console.WriteLine("Clients informed, removing player");
                    players.RemoveAt(i);
                    Console.WriteLine("Player removed");
                    Console.WriteLine();
                }
            }
        }

        private static void AkwardHandshaking()
        {
            while (listener.Pending())
            {
                Console.WriteLine();
                Console.WriteLine("Connection available...");
                Socket socket = listener.AcceptSocket();

                Console.WriteLine("Compare versions...");
                Console.WriteLine("Server version: " + version);
                socket.Send(new byte[] { 1,version });

                byte[] rec = new byte[2];
                socket.Receive(rec,2,SocketFlags.None);
                Console.WriteLine("Client version: " + rec);

                if (rec.Equals(version))
                {
                    Console.WriteLine("versions match");
                    InitialisePlayer(socket);
                }
            }
        }

        private static void InitialisePlayer(Socket socket)
        {
            Player player = new Player(socket,freeIDs.Pop());
            Console.Out.WriteLine("Player with ID:" + player.ID + "created");
            players.Add(player);
            pendingPlayers.Add(player);
        }

        private static void AddPendingPlayer(Player player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
                pendingPlayers.Add(player);
            }
            else
            {
                if (inLobyPlayers.Contains(player)) {
                    inLobyPlayers.Remove(player);
                    pendingPlayers.Add(player);
                }
                else if (!pendingPlayers.Contains(player))
                {
                    pendingPlayers.Add(player);
                }
            }
            
        }

        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            return !(part1 && part2);
        }
    }
}
