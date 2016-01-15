
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

    //玩家列表
    public class sendLogin_playerListVO
    {
        
        //战场唯一id
        public short battleId;
        //
        public short teamId;
        //
        public short actorId;
        //当前x
        public short curX;
        //当前y
        public short curY;
        //走路0，跑1
        public bool fast;
        
        
        public void decode(EndianBinaryReader binReader)
        {
        
            battleId = binReader.ReadInt16();
            
            teamId = binReader.ReadInt16();
            
            actorId = binReader.ReadInt16();
            
            curX = binReader.ReadInt16();
            
            curY = binReader.ReadInt16();
            
            fast = binReader.ReadBoolean();
            
        }
        
        public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.Write(battleId);
            
            binWriter.Write(teamId);
            
            binWriter.Write(actorId);
            
            binWriter.Write(curX);
            
            binWriter.Write(curY);
            
            binWriter.Write(fast);
            
        }
        
    }

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
            
            m_date = TimeUtils.SecondsToDateTime(binReader.ReadUInt32());
            
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
            
            binWriter.Write(TimeUtils.DateTimeToSeconds(m_date));
            
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

    //技能打击伤害
    public class testCommonStructVO : BaseVO
    {
        
        //战场唯一id
        public short battleId;
        //链接公用结构体
        public CommonStructVO[] damageList;
        
        
        public testCommonStructVO() : base(Protocol.testCommonStruct)
        {

        }
        
        override public void decode(EndianBinaryReader binReader)
        {
        
            battleId = binReader.ReadInt16();
            
            var len_damageList = binReader.ReadUInt16();
            damageList = new CommonStructVO[len_damageList];
            for (int i = 0; i < len_damageList; i++)
            {

            damageList[i] = new CommonStructVO();
            damageList[i].decode(binReader);
            
            }
            
        }
        
        override public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.Write(battleId);
            
			ushort len_damageList = (ushort)damageList.Length;
			binWriter.Write(len_damageList);
            for (int i = 0; i < len_damageList; i++)
            {

            damageList[i].encode(binWriter);
            
            }
            
        }
        
    }

    //
    public class sendStringVO : BaseVO
    {
        
        //
        public string str;
        
        
        public sendStringVO() : base(Protocol.sendString)
        {

        }
        
        override public void decode(EndianBinaryReader binReader)
        {
        
            str = binReader.ReadUTF();
            
        }
        
        override public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.WriteUTF(str);
            
        }
        
    }


}