using FreeNet;

namespace Protocol
{
    public class SCUserInfo : AProtocolMessage<SCUserInfo>
    {
        public short UserID { get; set; }

        public SCUserInfo(CPacket msg = null)
            : base(EPacketProtocol.USER_INFO)
        {
            if (null != msg)
                FromPacket(msg);
        }
        public override SCUserInfo FromPacket(CPacket msg)
        {
            UserID = msg.pop_int16();
            return this;
        }

        public override CPacket ToPacket()
        {
            var ret = CPacket.create(PacketProtocol.ToShort());
            ret.push(UserID);
            return ret;
        }

    }
}
