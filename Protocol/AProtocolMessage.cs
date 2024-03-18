using FreeNet;

namespace Protocol
{
    public abstract class AProtocolMessage<T> where T : AProtocolMessage<T>
    {
        protected EPacketProtocol PacketProtocol;

        public AProtocolMessage(EPacketProtocol protocol)
        {
            PacketProtocol = protocol;
        }

        public abstract CPacket ToPacket();

        public abstract T FromPacket(CPacket msg);

    }
}
