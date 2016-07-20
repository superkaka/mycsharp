// These Codes are generated by kakaTools ProtocolGenerater v1.4
// ------------------------------------------------------------------
//
// Copyright (c) 2010——2016 linchen.
// All rights reserved.
//
// Email: superkaka.org@gmail.com
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KLib;

namespace protocol
{
    /// <summary>
    /// 返回玩家列表
    /// </summary>
    public class ResponsePlayers : BaseProtocolVOGeneric<ResponsePlayers>
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        public bool status;
        
        /// <summary>
        /// 玩家列表
        /// </summary>
        public PlayerInfo[] players;
        
        
        public ResponsePlayers() : base(MessageType.ResponsePlayers)
        {

        }
        
        override public void decode(ProtocolBinaryReader binReader)
        {
        
            status = binReader.ReadBoolean();
            
            var len_players = binReader.ReadInt32();
            players = new PlayerInfo[len_players];
            for (int i = 0; i < len_players; i++)
            {
                players[i] = new PlayerInfo();
            players[i].decode(binReader);
            }
            
        }
        
        override public void encode(ProtocolBinaryWriter binWriter)
        {
        
            binWriter.Write(status);
            
            int len_players = players.Length;
            binWriter.Write(len_players);
            for (int i = 0; i < len_players; i++)
            {
                players[i].encode(binWriter);
            }
            
        }
        
    }

}
