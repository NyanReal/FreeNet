using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace CSampleServer
{
	class Program
	{
		static List<CGameUser> userlist;

		static ConcurrentDictionary<short,byte> userids = new ConcurrentDictionary<short, byte>();

		static void Main(string[] args)
		{
			userlist = new List<CGameUser>();

			CNetworkService service = new CNetworkService(false);
			// 콜백 매소드 설정.
			service.session_created_callback += on_session_created;
            // 초기화.
            service.initialize(10000, 1024);
			service.listen("0.0.0.0", 3369, 100);

            // 서버에서 하트비트 체크를 끌때 사용함.
            // 스트레스 테스트를 하기 위해 FreeNet이 아닌 다른 클라이언트를 쓰는 경우등에 필요할것 같다.
            // Remove below comments to disable heartbeat on server.
            // (It maybe use to stress test from another client program not using FreeNet.)
            service.disable_heartbeat();


			Console.WriteLine("Started!");
			while (true)
			{
                //Console.Write(".");
                string input = Console.ReadLine();
                if (input.Equals("users"))
                {
                    Console.WriteLine(service.usermanager.get_total_count());
                }
				System.Threading.Thread.Sleep(1000);
			}

			//Console.ReadKey();
		}

		/// <summary>
		/// 클라이언트가 접속 완료 하였을 때 호출됩니다.
		/// n개의 워커 스레드에서 호출될 수 있으므로 공유 자원 접근시 동기화 처리를 해줘야 합니다.
		/// </summary>
		/// <returns></returns>
		static void on_session_created(CUserToken token)
		{
            short newid = 0;
            lock (userids)
			{
                for (short i = 1; i <= short.MaxValue; i++)
                {
                    if (!userids.ContainsKey(i))
                    {                        
                        newid = i;
                        userids.TryAdd(newid, 0);
                        break;
                    }
                }
            }

            CGameUser user = new CGameUser(token, newid);

            lock (userlist)
            {
                userlist.Add(user);
            }
        }

		public static void remove_user(CGameUser user)
		{
            lock (userids)
            {
                userids.Remove(user.sig, out byte ret);
            }
            lock (userlist)
            {
                userlist.Remove(user);
            }
        }

		public static void SendAll(CPacket pkt)
		{
            foreach (CGameUser user in userlist)
            {
                if(userids.TryGetValue(user.sig, out byte ret))
                {
                    if(ret == 0)
                    {
                        CPacket msg = CPacket.create((short)GameServer.PROTOCOL.USER_INFO);
                        msg.push(user.sig);
                        user.send(msg);
                        if(!userids.TryUpdate(user.sig, 1, 0))
                        {
                            Console.WriteLine("id info send state update fail!");
                        }
                    }

                }

            }

            foreach (CGameUser user in userlist) 
			{ 
				user.send(pkt);
			}
		}
	}
}
