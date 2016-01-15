
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
    //
    public class sendLogin_testStructVO
    {
        
        //
        public ushort m_ushort;
        //
        public int m_int;
        //
        public uint m_uint;
        //
        public bool m_Boolean;
        //二进制类型
        public byte[] m_Binary;
        //
        public string m_string;
        //
        public long m_long;
        //
        public ulong m_ulong;
        
        
        public void decode(EndianBinaryReader binReader)
        {
        
            m_ushort = binReader.ReadUInt16();
            
            m_int = binReader.ReadInt32();
            
            m_uint = binReader.ReadUInt32();
            
            m_Boolean = binReader.ReadBoolean();
            
            m_Binary = binReader.ReadBytes(binReader.ReadInt32());
            
            m_string = binReader.ReadUTF();
            
            m_long = binReader.ReadInt64();
            
            m_ulong = binReader.ReadUInt64();
            
        }
        
        public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.Write(m_ushort);
            
            binWriter.Write(m_int);
            
            binWriter.Write(m_uint);
            
            binWriter.Write(m_Boolean);
            
            binWriter.Write(m_Binary.Length);
            binWriter.Write(m_Binary);
            
            binWriter.WriteUTF(m_string);
            
            binWriter.Write(m_long);
            
            binWriter.Write(m_ulong);
            
        }
        
    }

}
