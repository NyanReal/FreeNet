using System.Runtime.CompilerServices;

namespace Protocol
{
    public enum EPacketProtocol : short
    {
        BEGIN = 0,

        //CHAT_MSG_REQ = 1,
        //CHAT_MSG_ACK = 2,
        MOVE_REQ = 3,
        MOVE_CAST = 4,
        USER_INFO = 5,

        END,
    }


    public static class EPacketProtocolExtension
    {
        public static short ToShort(this EPacketProtocol protocol) => (short)protocol;

        public static EPacketProtocol ToProtocol(this short protocol) => (EPacketProtocol)protocol;

    }



}
