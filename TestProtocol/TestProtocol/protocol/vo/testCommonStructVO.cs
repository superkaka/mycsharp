
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
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

}
