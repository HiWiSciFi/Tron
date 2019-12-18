using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TronServerNeu
{

    public static class NetworkProtokoll
    {

        public static class ID
        {
            public const byte playerDisconect = 4;
            // 1 byte id

            public const byte standart = 0;
            // ?32? byte pos, rot, boosted

            public const byte info = 1;
            // 4 byte ID 1 byte, color 3 byte

            public const byte col = 2;
            // 1 byte ID

            public const byte kill = 3;
            // 1 byte ID



        }

        /// <summary>
        /// sends date whithout index
        /// header is automaticaly generated
        /// </summary>
        /// <param name="socket">socket to which data is sent</param>
        /// <param name="data">data sent</param>
        public static void Send(Socket socket, byte[] data)
        {
            
            byte[] buffer = new byte[data.Length + 1];
            buffer[0] = (byte)data.Length;
            Array.Copy(data,0,buffer,1,data.Length);

            socket.Send(buffer);
        }

        public static void Broadcast(List<Player> players, byte[] data)
        {
            for(int i = 0; i < players.Count; i++)
            {
                Send(players[i].socket, data);
            }
        }

        

        public static byte[] Receive(Socket socket)
        {
            
            byte[] header = new byte[1];
            socket.Receive(header,1,SocketFlags.None);

            byte[] data = new byte[header[0]];
            socket.Receive(data,header[0],SocketFlags.None);
            return data;
        }

        public static byte[][] SplitInformation(byte[] daten)
        {

            byte[] index = daten.Take(daten[1] - 1).ToArray();
            byte[] information = daten.Skip(daten[1] - 1).ToArray();

            byte[][] toReturn = new byte[index.Length / 2][];

            for (int i = 0; i < index.Length / 2; i++)
            {
                byte[] currentIndex = index
                    .Skip(i*2)
                    .ToArray()
                    .Take(2)
                    .ToArray();
                toReturn[i] = new byte[currentIndex[1] + 1];
                toReturn[i][0] = currentIndex[0];
            }
            return toReturn;
        }

    }


    


}