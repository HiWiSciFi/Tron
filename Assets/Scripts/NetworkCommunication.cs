using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Linq;

public static class NetworkCommunication
{
    static TcpClient client;
    static ASCIIEncoding asen = new ASCIIEncoding();
    static NetworkStream stream;
    public static Controller localPlayer;
    /// <summary>
    /// true, if a connection exists and data is available
    /// </summary>
    public static bool DataAvailable { get { return stream != null ? stream.DataAvailable : false; } }

    public static bool Connect(string IP, int port, int version, ref int ID)
    {
        client = new TcpClient();
        Debug.Log("Connecting...");
        client.Connect(IP, port);
        Debug.Log("Connected");

        stream = client.GetStream();

        Debug.Log("Matching versions...");
        byte[] vB = new byte[4];
        stream.Read(vB, 0, 4);
        int v = BitConverter.ToInt32(vB, 0);
        Debug.Log("Client version is " + version);
        Debug.Log("Server version is " + v);
        stream.Write(BitConverter.GetBytes(version), 0, 4);

        if (v != version)
        {
            Debug.LogWarning("Game version does not matching server version");
            return false;
        }

        Debug.Log("Server and Client version matching");
        Debug.Log("Request Player ID...");
        byte[] b = new byte[4];
        stream.Read(b, 0, 4);
        ID = BitConverter.ToInt32(b, 0);
        Debug.Log("new ID is " + ID);

        Debug.Log("Requesting other players...");
        b = new byte[4];
        stream.Read(b, 0, 4);
        int playerAmount = BitConverter.ToInt32(b, 0);
        Debug.Log("There are " + playerAmount + " players to add");
        for (int i = 0; i < playerAmount; i++)
        {
            b = new byte[4];
            stream.Read(b, 0, 4);
            int newID = BitConverter.ToInt32(b, 0);
            Debug.Log("Adding player with ID " + newID);
        }

        return true;
    }

    public static void Disconnect()
    {
        client.Close();
        stream = null;
        client = null;
    }

    public static void ReadData()
    {
        while (DataAvailable)
        {
            byte[] index = new byte[1];
            stream.Read(index, 0, 1);
            if (index[0] == 0)
            {
                //get information about another player
                byte[] received = new byte[24];
                stream.Read(received, 0, 24);
                int ID = BitConverter.ToInt32(received, 0);
                Controller[] players = localPlayer.getAllPlayers();
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].ID == ID)
                    {

                    }
                }
            }
            else if (index[0] == 1)
            {
                //Player Connected
                Debug.Log("Player Connected");
                byte[] received = new byte[4];
                stream.Read(received, 0, 4);
                int addedPlayerID = BitConverter.ToInt32(received, 0);
                Debug.Log("Adding Player " + addedPlayerID);
            }
            else if (index[0] == 2)
            {
                //Player Disconnected
                Debug.Log("Player Disconnected");
                byte[] received = new byte[4];
                stream.Read(received, 0, 4);
                int leftPlayerID = BitConverter.ToInt32(received, 0);
                Debug.Log("Removing Player " + leftPlayerID);
            }
            else if (index[0] == 3)
            {
                //Ready for Data
                stream.Write(new byte[] { 0 }, 0, 1);

                //send posX, posZ, rotY, rotW
                Transform data = localPlayer.gameObject.transform;
                Rigidbody datarb = localPlayer.gameObject.GetComponent<Rigidbody>();
                byte[] posX = BitConverter.GetBytes(data.position.x);
                byte[] posZ = BitConverter.GetBytes(data.position.z);
                byte[] rotY = BitConverter.GetBytes(data.rotation.y);
                byte[] rotW = BitConverter.GetBytes(data.rotation.w);
                byte[] velX = BitConverter.GetBytes(datarb.velocity.x);
                byte[] velZ = BitConverter.GetBytes(datarb.velocity.z);
                byte[] toSend = new byte[24];
                for (int i = 0; i < toSend.Length; i++)
                {
                    if (i < 4)
                        toSend[i] = posX[i];
                    else if (i < 8)
                        toSend[i] = posZ[i];
                    else if (i < 12)
                        toSend[i] = rotY[i];
                    else if (i < 16)
                        toSend[i] = rotW[i];
                    else if (i < 20)
                        toSend[i] = velX[i];
                    else if (i < 24)
                        toSend[i] = velZ[i];
                }
                stream.Write(toSend, 0, 16);
            }
        }
    }

    public static void SendLinePos(byte ID, Vector3 point)
    {

    }
}