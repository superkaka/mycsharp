using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib.utils;
using protocol.vo;

namespace protocol
{

    public interface IProtocolVOCreater
    {
        BaseVO CreateProtocolVO(int id);
    }

    public class PackageTranslator
    {

        private IProtocolVOCreater voCreater;

        public PackageTranslator(IProtocolVOCreater voCreater)
        {
            this.voCreater = voCreater;
        }

        public byte[] Encode(BaseVO vo)
        {
            var binWriter = new EndianBinaryWriter(Endian.BigEndian, new MemoryStream());
            binWriter.Write(vo.ProtocolId);
            vo.encode(binWriter);
            binWriter.Seek(0, SeekOrigin.Begin);
            int len = (int)binWriter.BaseStream.Length;
            var bytes = new byte[len];
            binWriter.BaseStream.Read(bytes, 0, len);
            return bytes;
        }

        public BaseVO Decode(byte[] bytes)
        {
            var binReader = new EndianBinaryReader(Endian.BigEndian, new MemoryStream(bytes));
            var id = binReader.ReadInt32();

            var vo = voCreater.CreateProtocolVO(id);
            vo.decode(binReader);

            return vo;
        }

    }
}