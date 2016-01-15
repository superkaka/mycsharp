
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
    //登录
    public class sendLoginVO : BaseVO
    {
        
        //
        public short actorId;
        //
        public sbyte m_byte;
        //
        public DateTime m_date;
        //
        public double m_float;
        //
        public sendLogin_testStructVO testStruct;
        //玩家列表
        public sendLogin_playerListVO[] playerList;
        
        
        public sendLoginVO() : base(Protocol.sendLogin)
        {

        }
        
        override public void decode(EndianBinaryReader binReader)
        {
        
            actorId = binReader.ReadInt16();
            
            m_byte = binReader.ReadSByte();
            
            m_date = binReader.ReadDate();
            
            m_float = binReader.ReadFloat();
            
            testStruct = new sendLogin_testStructVO();
            testStruct.decode(binReader);
            
            var len_playerList = binReader.ReadUInt16();
            playerList = new sendLogin_playerListVO[len_playerList];
            for (int i = 0; i < len_playerList; i++)
            {

            playerList[i] = new sendLogin_playerListVO();
            playerList[i].decode(binReader);
            
            }
            
        }
        
        override public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.Write(actorId);
            
            binWriter.Write(m_byte);
            
            binWriter.WriteDate(m_date);
            
            binWriter.WriteFloat(m_float);
            
            testStruct.encode(binWriter);
            
			ushort len_playerList = (ushort)playerList.Length;
			binWriter.Write(len_playerList);
            for (int i = 0; i < len_playerList; i++)
            {

            playerList[i].encode(binWriter);
            
            }
            
        }
        
    }

}
