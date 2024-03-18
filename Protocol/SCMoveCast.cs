using FreeNet;

namespace Protocol
{
    public class SCMoveCast : AProtocolMessage<SCMoveCast>
    {
        public short UserID { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }


        public SCMoveCast(CPacket msg = null)
            : base(EPacketProtocol.MOVE_CAST)
        {
            if (null != msg)
                FromPacket(msg);
        }
        public override SCMoveCast FromPacket(CPacket msg)
        {
            UserID = msg.pop_int16();
            X = msg.pop_float();
            Y = msg.pop_float();
            Z = msg.pop_float();
            Rotation = msg.pop_float();
            return this;
        }

        public override CPacket ToPacket()
        {
            var msg = CPacket.create(PacketProtocol.ToShort());
            msg.push(UserID);
            msg.push(X);
            msg.push(Y);
            msg.push(Z);
            msg.push(Rotation);
            return msg;
        }

        public override string ToString()
        {
            return $"{PacketProtocol} User ({UserID}) Pos ({X}, {Y}, {Z}) rot : {Rotation} deg";
        }

    }
}
