using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TronServer
{
    public class Player
    {
        public Socket socket;

        public byte[] bID;
        public int ID;

        public byte[] bposX, bposZ;
        public byte[] brotY, brotW;
        public byte[] bvelX, bvelZ;

        public float posX, posZ;
        public float rotY, rotW;
        public float velX, velZ;

        public Player(Socket socket, int ID)
        {
            this.socket = socket;
            this.ID = ID;
            IDToByteArray();
        }

        public void byteArraysToFloats()
        {
            posX = BitConverter.ToSingle(bposX, 0);
            posZ = BitConverter.ToSingle(bposZ, 0);
            rotY = BitConverter.ToSingle(brotY, 0);
            rotW = BitConverter.ToSingle(brotW, 0);
            velX = BitConverter.ToSingle(bvelX, 0);
            velZ = BitConverter.ToSingle(bvelZ, 0);
        }

        public void IDToByteArray()
        {
            bID = BitConverter.GetBytes(ID);
        }

        public byte[] toOneArray()
        {
            byte[] toReturn = new byte[28];
            for (int i = 0; i < toReturn.Length; i++)
            {
                if (i < 4)
                    toReturn[i] = bID[i];
                else if (i < 8)
                    toReturn[i] = bposX[i];
                else if (i < 12)
                    toReturn[i] = bposZ[i];
                else if (i < 16)
                    toReturn[i] = brotY[i];
                else if (i < 20)
                    toReturn[i] = brotW[i];
                else if (i < 24)
                    toReturn[i] = bvelX[i];
                else if (i < 28)
                    toReturn[i] = bvelZ[i];
            }
            return toReturn;
        }
    }
}
