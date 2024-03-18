using System;
using FreeNet;
using Protocol;

namespace CSampleServer
{
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
            var protocol = msg.pop_protocol_id().ToProtocol();
            Console.WriteLine("------------------------------------------------------ protocol id " + protocol);

            switch (protocol)
            {
                //case EPacketProtocol.CHAT_MSG_REQ:
                //    ProcChat(msg);
                //    break;
                case EPacketProtocol.MOVE_REQ:
                    ProcMove(msg);
                    break;
                default:
                    break;
            }
        }

        private void ProcMove(CPacket msg)
        {
            var moveReq = new CSMoveReq(msg);
            var ret = new SCMoveCast();
            ret.UserID = sig;
            ret.X = moveReq.X;
            ret.Y = moveReq.Y;
            ret.Z = moveReq.Z;
            ret.Rotation = moveReq.Rotation;

            Console.WriteLine(ret.ToString());

            Program.SendAll(ret.ToPacket());

            //short userid = sig;
            //float x = msg.pop_float();
            //float y = msg.pop_float();
            //float z = msg.pop_float();
            //float r = msg.pop_float();
            //Console.WriteLine($"move {sig} {x} {y} {z} {r}");
            //CPacket response = CPacket.create((short)EPacketProtocol.MOVE_CAST);
            //response.push(userid);
            //response.push(x);
            //response.push(y);
            //response.push(z);
            //response.push(r);
            //Program.SendAll(response);
        }

        //private static void ProcChat(CPacket msg)
        //{
        //    string text = msg.pop_string();
        //    Console.WriteLine(string.Format("text {0}", text));

        //    CPacket response = CPacket.create((short)EPacketProtocol.CHAT_MSG_ACK);
        //    response.push(text);
        //    //send(response);

        //    Program.SendAll(response);

        //    //if (text.Equals("exit"))
        //    //{                            
        //    //    for (int i = 0; i < 1000; ++i)
        //    //    {
        //    //        CPacket dummy = CPacket.create((short)PROTOCOL.CHAT_MSG_ACK);
        //    //        dummy.push(i.ToString());
        //    //        send(dummy);
        //    //    }
        //    //    this.token.ban();
        //    //}
        //}
    }
}
