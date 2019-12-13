using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TronServer
{
    class Program
    {
        private static bool portFound = false;
        private static int port = 0;
        private static bool versionFound = false;
        private static int version = 0;
        private static IPAddress ipAd;
        private static TcpListener listener;
        private static List<Player> connectedPlayers;

        //args:
        //listenport:<port>
        //gameversion:<version>
        static void Main(string[] args)
        {
            ProcessArgs(args);
            StartServer();

            if (portFound && versionFound)
            {
                while (true)
                {
                    HandleConnections();

                    //ready players to receive
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        connectedPlayers[i].socket.Send(new byte[] { 3 });
                        connectedPlayers[i].socket.Receive(new byte[1], SocketFlags.None);
                    }
                    //players ready
                    //ask players for info
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        connectedPlayers[i].socket.Send(new byte[] { 1 }, SocketFlags.None);
                        //posX, posZ, rot Y, rotW
                        byte[] b = new byte[24];
                        connectedPlayers[i].socket.Receive(b, SocketFlags.None);
                        connectedPlayers[i].bposX = b.Take(4).ToArray();
                        connectedPlayers[i].bposZ = b.Skip(4).Take(4).ToArray();
                        connectedPlayers[i].brotY = b.Skip(8).Take(4).ToArray();
                        connectedPlayers[i].brotW = b.Skip(12).Take(4).ToArray();
                        connectedPlayers[i].bvelX = b.Skip(16).Take(4).ToArray();
                        connectedPlayers[i].bvelZ = b.Skip(20).Take(4).ToArray();
                        connectedPlayers[i].byteArraysToFloats();
                    }

                    //send info to all players
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        byte[] toSend = connectedPlayers[i].toOneArray();
                        for (int j = 0; j < connectedPlayers.Count; j++)
                        {
                            if (j == i)
                                continue;
                            connectedPlayers[j].socket.Send(new byte[] { 0 }, SocketFlags.None);
                            connectedPlayers[j].socket.Send(toSend, SocketFlags.None);
                        }
                    }
                }
            }
            //exit if while loop ends
            Environment.Exit(0);
        }

        private static void ProcessArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().StartsWith("listenport:"))
                {
                    try
                    {
                        string[] split = args[i].Split(':');
                        port = int.Parse(split[1]);
                        portFound = true;
                        continue;
                    }
                    catch { }
                }

                if (args[i].ToLower().StartsWith("gameversion:"))
                {
                    try
                    {
                        string[] split = args[i].Split(':');
                        version = int.Parse(split[1]);
                        versionFound = true;
                        continue;
                    }
                    catch { }
                }
            }
        }

        private static void StartServer()
        {
            ipAd = IPAddress.Any;
            Console.WriteLine("Starting listener on port " + port + " using IP " + ipAd);
            listener = new TcpListener(ipAd, port);

            listener.Start();

            Console.WriteLine("Server running on port " + port);

            connectedPlayers = new List<Player>();

            Console.WriteLine("Server started");
            Console.WriteLine("----------------------------------");
        }

        private static void HandleConnections()
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (!SocketConnected(connectedPlayers[i].socket))
                {
                    Console.WriteLine();
                    Console.WriteLine("Player " + connectedPlayers[i].ID + " Disconnected");
                    Console.WriteLine("Informing Clients");
                    for (int j = 0; j < connectedPlayers.Count; j++)
                    {
                        if (j != i)
                        {
                            connectedPlayers[j].socket.Send(new byte[] { 2 }, 1, SocketFlags.None);
                            connectedPlayers[j].socket.Send(BitConverter.GetBytes(connectedPlayers[i].ID), 4, SocketFlags.None);
                        }
                    }
                    Console.WriteLine("Clients informed, removing player");
                    connectedPlayers.RemoveAt(i);
                    Console.WriteLine("Player removed");
                    Console.WriteLine();
                }
            }

            while (listener.Pending())
            {
                Console.WriteLine();
                Console.WriteLine("Connection available...");
                Socket socket = listener.AcceptSocket();

                Console.WriteLine("Compare versions...");
                Console.WriteLine("Server version: " + version);
                socket.Send(BitConverter.GetBytes(version));
                byte[] recB = new byte[4];
                socket.Receive(recB, 0, 4, SocketFlags.None);
                int rec = BitConverter.ToInt32(recB, 0);
                Console.WriteLine("Client version: " + rec);
                if (rec == version)
                {
                    Console.WriteLine("versions match");

                    int playerID = GetNewID();
                    socket.Send(BitConverter.GetBytes(playerID));
                    Console.WriteLine("Assigned new ID: " + playerID);

                    Console.WriteLine("Sending current players...");
                    socket.Send(BitConverter.GetBytes(connectedPlayers.Count));
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        socket.Send(BitConverter.GetBytes(connectedPlayers[i].ID));
                    }

                    Console.WriteLine("Informing Clients...");
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        connectedPlayers[i].socket.Send(new byte[] { 1 });
                        connectedPlayers[i].socket.Send(BitConverter.GetBytes(playerID));
                    }
                    Console.WriteLine("Clients Informed");

                    connectedPlayers.Add(new Player(socket, playerID));
                    Console.WriteLine("New Player connected");
                }
                else
                {
                    Console.WriteLine("versions not matching, cancelling connection");
                }
                Console.WriteLine();
            }
        }

        private static int GetNewID()
        {
            for (int i = 0; i < connectedPlayers.Count+1; i++)
            {
                for (int j = 0; j < connectedPlayers.Count; j++)
                {
                    if (connectedPlayers[j].ID == i)
                    {
                        break;
                    }
                }
            }
            return -1;
        }

        private static bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}