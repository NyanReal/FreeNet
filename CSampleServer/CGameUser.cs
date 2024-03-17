using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace CSampleServer
{
	using GameServer;
    using System.Reflection.Metadata;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 하나의 session객체를 나타낸다.
    /// </summary>
    class CGameUser : IPeer
	{
        public short sig { get; private set; }

        CUserToken token;

		public CGameUser(CUserToken token, short sig)
		{
			this.token = token;
            this.sig = sig;
			this.token.set_peer(this);
		}

		void IPeer.on_removed()
		{
			//Console.WriteLine("The client disconnected.");

			Program.remove_user(this);
		}

		public void send(CPacket msg)
		{
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
		}

        public void send(ArraySegment<byte> data)
        {
            this.token.send(data);
        }

		void IPeer.disconnect()
		{
            this.token.ban();
		}

		void IPeer.on_message(CPacket msg)
		{
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            Console.WriteLine("------------------------------------------------------ protocol id " + protocol);

            switch (protocol)
            {
                case PROTOCOL.CHAT_MSG_REQ:
                    ProcChat(msg);
                    break;
                case PROTOCOL.MOVE_REQ:
                    ProcMove(msg);
                    break;
            }
        }

        private void ProcMove(CPacket msg)
        {
            short userid = sig;
            float x = msg.pop_float();
            float y = msg.pop_float();
            float z = msg.pop_float();
            float r = msg.pop_float();
            Console.WriteLine($"move {sig} {x} {y} {z} {r}");
            CPacket response = CPacket.create((short)PROTOCOL.MOVE_CAST);
            response.push(userid);
            response.push(x);
            response.push(y);
            response.push(z);
            response.push(r);
            Program.SendAll(response);
        }

        private static void ProcChat(CPacket msg)
        {
            string text = msg.pop_string();
            Console.WriteLine(string.Format("text {0}", text));

            CPacket response = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
            response.push(text);
            //send(response);

            Program.SendAll(response);

            //if (text.Equals("exit"))
            //{                            
            //    for (int i = 0; i < 1000; ++i)
            //    {
            //        CPacket dummy = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
            //        dummy.push(i.ToString());
            //        send(dummy);
            //    }
            //    this.token.ban();
            //}
        }
    }
}
