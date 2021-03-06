using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public static class NetworkCommunication {

	private static TcpClient client;
	private static NetworkStream stream;
	public const byte VERSION = 1;

	public static bool DataAvailable { get { return stream != null ? stream.DataAvailable : false; } }

	/// <summary>
	/// disconnect from server
	/// </summary>
	public static void Disconnect()
	{
		stream.Close();
		stream.Dispose();
		client.Close();
		stream = null;
		client = null;
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// Connects to a server and does the handshake
	/// </summary>
	/// <param name="IP">The server hostname</param>
	/// <param name="PORT">The server Port</param>
	/// <returns>Error Codes: 0 = everthing fine, 1 = could not connect, 2 = versions not matching</returns>
	public static int Connect(string IP, int PORT, out Color color, out byte ID) {
		try {
			Debug.Log("Connecting to " + IP + " at " + PORT + "...");
			client = new TcpClient();
			client.Connect(IP, PORT);
			stream = client.GetStream();
			Debug.Log("Connected");
		} catch {
			Debug.LogError("Could not connect to " + IP + " at " + PORT);
			color = Color.black;
			ID = 0;
			return 1;
		}

		// handshake
		while (!DataAvailable);
		byte version = Receive()[0];
		stream.Write(new byte[] { 1, VERSION }, 0, 2);

		if (version != VERSION)
		{
			Debug.LogError("Server and client versions not matching - Server: " + version + " Client: " + VERSION);
			color = Color.black;
			ID = 0;
			return 2;
		}
		else
		{
			Debug.Log("Versions matching");
		}

		while (!DataAvailable);
		byte[] buffer = Receive();
		ID = buffer[1];
		Debug.Log("ID " + ID + " assigned to local player");
		color = new Color(buffer[2], buffer[3], buffer[4]);
		Debug.Log("Color " + color.r + " " + color.g + " " + color.b + " assigned to local player");

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
		information.AddRange(BitConverter.GetBytes(info.transform.rotation.y));
		information.AddRange(BitConverter.GetBytes(info.transform.position.x));
		information.AddRange(BitConverter.GetBytes(info.transform.position.z));
		information.Add(info.boosted);

		// compile Lists to data package
		List<byte> package = new List<byte>();
		package.Add((byte)(IIDs.Count + information.Count));
		package.AddRange(IIDs);
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
}