using System;
using System.IO;
using KLib.utils;

namespace protocol
{
    public abstract class BaseProtocolVO
    {

        private MessageType protocolType;
        private int protocolId;

        public BaseProtocolVO(MessageType protocolType)
        {
            this.protocolType = protocolType;
            this.protocolId = (int)protocolType;
        }

        public MessageType ProtocolType
        {
            get { return protocolType; }
        }

        public int ProtocolId
        {
            get { return protocolId; }
        }

        public void decode(byte[] bytes)
        {
            var binReader = new EndianBinaryReader(Endian.BigEndian, new MemoryStream(bytes));
            decode(binReader);
        }

        //子类覆写
        public virtual void decode(EndianBinaryReader binReader)
        {

        }

        //子类覆写
        public virtual void encode(EndianBinaryWriter binWriter)
        {

        }

    }

}