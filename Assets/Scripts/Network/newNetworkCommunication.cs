using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Linq;

public static class newNetworkCommunication {

	private static TcpClient client;
	private static NetworkStream stream;
	public const byte VERSION = 1;

	public static bool DataAvailable { get { return stream != null ? stream.DataAvailable : false; } }

	/// <summary>
	/// Connects to a server and does the handshake
	/// </summary>
	/// <param name="IP">The server hostname</param>
	/// <param name="PORT">The server Port</param>
	/// <returns>Error Codes: 0 = everthing fine, 1 = could not connect, 2 = versions not matching</returns>
	public static int Connect(string IP, int PORT) {
		try {
			Debug.Log("Connecting to " + IP + " at " + PORT + "...");
			client = new TcpClient();
			client.Connect(IP, PORT);
			stream = client.GetStream();
			Debug.Log("Connected");
		} catch {
			Debug.LogError("Could not connect to " + IP + " at " + PORT);
			return 1;
		}

		// handshake
		while (!DataAvailable);
		byte version = Receive()[0];
		stream.Write(new byte[] { 1, VERSION }, 0, 2);

		if (version != VERSION)
		{
			Debug.LogError("Server and client versions not matching - Server: " + version + " Client: " + VERSION);
			return 2;
		}
		else
		{
			Debug.Log("Versions matching");
		}

		Debug.Log("Handshake successful");
		return 0;
	}

	/// <summary>
	/// Send the standard data package to server
	/// </summary>
	/// <param name="info">The local playerController</param>
	public static void SendUpdate(PlayerController info)
	{
		List<byte> IIDs = new List<byte>();
		List<byte> information = new List<byte>();

		// IID for standard data pack
		IIDs.Add(0);

		// actual data
		information.AddRange(BitConverter.GetBytes(info.transform.rotation.eulerAngles.y));
		information.AddRange(BitConverter.GetBytes(info.transform.position.x));
		information.AddRange(BitConverter.GetBytes(info.transform.position.z));
		information.Add(info.boosted);

		// compile Lists to data package
		List<byte> package = new List<byte>();
		package.Add((byte)(IIDs.Count + information.Count));
		package.AddRange(IIDs);
		package.Add(2);
		package.AddRange(information);

		stream.Write(package.ToArray(), 0, package.Count);
	}

	public static byte[] Receive()
	{
		byte[] header = new byte[1];
		stream.Read(header, 0, 1);

		byte[] data = new byte[header[0]];
		stream.Read(data, 0, header[0]);
		
		return data;
	}

	public static byte[][] SplitInformation(byte[] data)
	{
		byte[] index = data.Take(data[1] - 1).ToArray();
		byte[] information = data.Skip(data[1] - 1).ToArray();

		byte[][] toReturn = new byte[index.Length / 2][];

		for (int i= 0; i < index.Length / 2; i++)
		{
			byte[] currentIndex = index.Skip(i * 2).ToArray().Take(2).ToArray();
			toReturn[i] = new byte[currentIndex[1] + 1];
			toReturn[i][0] = currentIndex[0];
		}
		return toReturn;
	}
}