
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
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

}
