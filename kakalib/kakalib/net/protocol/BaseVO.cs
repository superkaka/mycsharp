using System;
using System.IO;
using KLib.utils;

namespace protocol
{
    public abstract class BaseVO
    {

        private int protocolId;

        public BaseVO(int protocolId)
        {
            this.protocolId = protocolId;
        }

        public int ProtocolId
        {
            get { return protocolId; }
        }

        public void decode(Byte[] bytes)
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