using FreeNet;

namespace Protocol
{
    public class CSMoveReq : AProtocolMessage<CSMoveReq>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Rotation { get; set; }

        public CSMoveReq(CPacket msg = null)
            : base(EPacketProtocol.MOVE_REQ)
        {
            if (null != msg)
                FromPacket(msg);
        }
        public override CSMoveReq FromPacket(CPacket msg)
        {
            X = msg.pop_float();
            Y = msg.pop_float();
            Z = msg.pop_float();
            Rotation = msg.pop_float();

            return this;
        }

        public override CPacket ToPacket()
        {
            CPacket msg = CPacket.create(PacketProtocol.ToShort());

            msg.push(X);
            msg.push(Y);
            msg.push(Z);
            msg.push(Rotation);

            return msg;
        }

        public override string ToString()
        {
            return $"{PacketProtocol} Pos ({X}, {Y}, {Z}) rot : {Rotation} deg";
        }

    }
}
