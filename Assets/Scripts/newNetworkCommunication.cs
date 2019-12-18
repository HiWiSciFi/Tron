using System;
using System.Net;
using System.Net.Sockets;

public static class newNetworkCommunication {
	
	static TcpClient client;
	
	public static bool Connect(string IP, int PORT) {
		try {
			client = new TcpClient();
			client.Connect(IP, PORT);
		} catch {
			
			return false;
		}
		
		//handshake
		
		
		return true;
	}
}