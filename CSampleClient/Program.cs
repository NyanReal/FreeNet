using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using FreeNet;

namespace CSampleClient
{
    using Protocol;

    class Program
	{
		static List<IPeer> game_servers = new List<IPeer>();

		static void Main(string[] args)
		{
			CPacketBufferManager.initialize(2000);
			// CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.
			// 메시지 송,수신은 서버, 클라이언트 모두 동일한 로직으로 처리될 수 있으므로
			// CNetworkService객체를 생성하여 Connector객체에 넘겨준다.
			CNetworkService service = new CNetworkService(true);

			// endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.
			CConnector connector = new CConnector(service);
			// 접속 성공시 호출될 콜백 매소드 지정.

			var uri = "sam.gamebass.net";
            var addresses = Dns.GetHostAddresses(uri);

			foreach( var address in addresses )
			{
				Console.WriteLine(address.ToString());
			}

            connector.connected_callback += on_connected_gameserver;
            IPEndPoint endpoint = new IPEndPoint(addresses[0], 3369);
			connector.connect(endpoint);
			//System.Threading.Thread.Sleep(10);

			while (true)
			{
				Console.Write("> ");
				string line = Console.ReadLine();
				if (line == "q")
				{
					break;
				}

				if (line.StartsWith("move"))
				{
					CPacket msg = CPacket.create((short)EPacketProtocol.MOVE_REQ);
					msg.push(1.0f); // x
					msg.push(2.0f); // y 
					msg.push(3.0f); // z
					msg.push(4.0f); // r
					game_servers[0].send(msg);
				}
				else
				{
					//CPacket msg = CPacket.create((short)EPacketProtocol.CHAT_MSG_REQ);
					//msg.push(line);
					//game_servers[0].send(msg);
				}
			}

			((CRemoteServerPeer)game_servers[0]).token.disconnect();

			//System.Threading.Thread.Sleep(1000 * 20);
			Console.ReadKey();
		}

		/// <summary>
		/// 접속 성공시 호출될 콜백 매소드.
		/// </summary>
		/// <param name="server_token"></param>
		static void on_connected_gameserver(CUserToken server_token)
		{
			lock (game_servers)
			{
				IPeer server = new CRemoteServerPeer(server_token);
                server_token.on_connected();
				game_servers.Add(server);
				Console.WriteLine("Connected!");
			}
		}
	}
}
