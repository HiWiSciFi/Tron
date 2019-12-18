using System;
using System.Net;
using System.Net.Sockets;

public static class NetworkCommunication {
	
	TcpClient client;
	
	public static bool Connect(string IP, string PORT) {
		try {
			client = new TcpClient(IP, PORT);
			client.Connect();
		} catch {
			
			return false;
		}
		
		//handshake
		
		
		return true;
	}
}