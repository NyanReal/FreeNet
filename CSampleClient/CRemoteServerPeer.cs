using System;
using FreeNet;
using Protocol;

namespace CSampleClient
{

    class CRemoteServerPeer : IPeer
	{
		public CUserToken token { get; private set; }

		public CRemoteServerPeer(CUserToken token)
		{
			this.token = token;
			this.token.set_peer(this);
		}

        int recv_count = 0;
		void IPeer.on_message(CPacket msg)
		{
            System.Threading.Interlocked.Increment(ref this.recv_count);

			EPacketProtocol protocol_id = msg.pop_protocol_id().ToProtocol();
			switch (protocol_id)
			{
				//case EPacketProtocol.CHAT_MSG_ACK:
				//	{
				//		string text = msg.pop_string();
				//		Console.WriteLine(string.Format("받 text {0}", text));
				//	}
    //                break;
                case EPacketProtocol.USER_INFO:
                    {
                        short id = msg.pop_int16();
                        Console.WriteLine(string.Format("yourid {0}", id));
                    }
                    break;
                case EPacketProtocol.MOVE_CAST:
                    {
                        SCMoveCast ret = new SCMoveCast(msg);
                        Console.WriteLine(ret.ToString());
                        //short userid = msg.pop_int16();
                        //float x = msg.pop_float();
                        //float y = msg.pop_float();
                        //float z = msg.pop_float();
                        //float r = msg.pop_float();
                        //Console.WriteLine($"move {userid} {x} {y} {z} {r}");
                    }
                    break;
                default:
                    break;
			}
		}

		void IPeer.on_removed()
		{
			Console.WriteLine("Server removed.");
            Console.WriteLine("recv count " + this.recv_count);
        }

		void IPeer.send(CPacket msg)
		{
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
		}

		void IPeer.disconnect()
		{
            this.token.disconnect();
		}
	}
}
