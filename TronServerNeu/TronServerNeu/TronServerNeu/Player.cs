using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TronServerNeu
{
    public class Player
    {
        public Socket socket;

        public byte ID;

        public float posX, posZ;
        public float rotY, rotW;
        public float velX, velZ;

        public Player(Socket socket, byte ID)
        {
            this.socket = socket;
            this.ID = ID;
        }
    }
}
