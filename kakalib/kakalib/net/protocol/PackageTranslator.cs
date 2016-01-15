using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib.utils;

namespace protocol
{

    public class PackageTranslator
    {

        public delegate BaseVO PackageCreate();

        private Dictionary<int, PackageCreate> dic_vo = new Dictionary<int, PackageCreate>();

        public PackageTranslator()
        {
            
        }

        public void RegisterMessage(int protocolId, PackageCreate packageCreateFun)
        {
            dic_vo[protocolId] = packageCreateFun;
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

            var packageCreateFun = dic_vo[id];

            var vo = packageCreateFun();
            vo.decode(binReader);

            return vo;
        }

    }
}