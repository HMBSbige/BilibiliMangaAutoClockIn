using System;
using System.Net;
using System.Net.Sockets;

namespace BilibiliApi
{
	public static class Ntp
	{
		/// <returns>Utc</returns>
		public static DateTime GetWebTime(IPEndPoint ntpServer)
		{
			// NTP message size - 16 bytes of the digest (RFC 2030)
			var ntpData = new byte[48];
			// Setting the Leap Indicator, Version Number and Mode values
			ntpData[0] = 0x1B; // LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.Connect(ntpServer);
			socket.ReceiveTimeout = 3000;
			socket.Send(ntpData);
			socket.Receive(ntpData);
			socket.Close();

			// Offset to get to the "Transmit Timestamp" field (time at which the reply 
			// departed the server for the client, in 64-bit timestamp format."
			const byte serverReplyTime = 40;
			ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);
			ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);
			intPart = ConvertEndian(intPart);
			fractPart = ConvertEndian(fractPart);
			var milliseconds = intPart * 1000 + fractPart * 1000 / 0x100000000UL;

			return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
		}

		private static uint ConvertEndian(ulong x)
		{
			var bData = BitConverter.GetBytes(Convert.ToUInt32(x));
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bData);
			}

			return BitConverter.ToUInt32(bData, 0);
		}
	}
}
