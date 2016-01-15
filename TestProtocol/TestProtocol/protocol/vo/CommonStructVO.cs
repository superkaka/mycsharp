
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
    //公用结构体
    public class CommonStructVO
    {
        
        //
        public ushort m_ushort;
        //
        public int m_int;
        //
        public uint m_uint;
        //
        public bool m_Boolean;
        //
        public byte[] m_Binary;
        //
        public string m_string;
        
        
        public void decode(EndianBinaryReader binReader)
        {
        
            m_ushort = binReader.ReadUInt16();
            
            m_int = binReader.ReadInt32();
            
            m_uint = binReader.ReadUInt32();
            
            m_Boolean = binReader.ReadBoolean();
            
            m_Binary = binReader.ReadBytes(binReader.ReadInt32());
            
            m_string = binReader.ReadUTF();
            
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
            
        }
        
    }

}