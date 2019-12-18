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
            public const byte playerDisconect = 0;
            public static class PlayerDisconect
            {
                public const byte cause1 = 0;
            }

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

        public static byte[] Receive(Socket socket)
        {
           List<ArraySegment<byte>> buffer = new List<ArraySegment<byte>>();
            
            socket.Receive(buffer);
            
            byte[] toReturn = new byte[buffer.Count];
            for (int i = 0; i < buffer.Count; i++)
            {
                toReturn[i] = buffer[i].Array.ToArray<byte>()[0];
            }


            return toReturn;
        }

    }


    


}