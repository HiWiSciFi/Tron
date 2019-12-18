using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;

public static class newNetworkCommunication {
	
	private static TcpClient client;
	private static NetworkStream stream;

	public static bool DataAvailable { get { return stream != null ? stream.DataAvailable : false; } }

	/// <summary>
	/// Connects to a server and does the handshake
	/// </summary>
	/// <param name="IP">The server hostname</param>
	/// <param name="PORT">The server Port</param>
	/// <returns>Error Codes: 0 = everthing fine, 1 = could not connect, 2 = versions not matching</returns>
	public static int Connect(string IP, int PORT) {
		try {
			client = new TcpClient();
			client.Connect(IP, PORT);
			stream = client.GetStream();
		} catch {
			
			return 1;
		}
		
		//handshake
		
		
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
		package.AddRange(information);

		stream.Write(package.ToArray(), 0, package.Count);
	}
}