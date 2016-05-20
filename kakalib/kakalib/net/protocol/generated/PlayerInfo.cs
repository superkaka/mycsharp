// These Codes are generated by kakaTools ProtocolGenerater v1.3
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
using KLib.utils;

namespace protocol.vo
{
    //
    public class PlayerInfo : BaseProtocolVO
    {
        
        static public PlayerInfo CreateInstance()
        {
            return new PlayerInfo();
        }
        
        //
        public long uid;
        //名字
        public string name;
        //状态
        public bool status;
        //类型
        public PlayerType type;
        //最大可重置次数（根据VIP等级计算）
        public int maxResetTimes;
        //道具列表
        public int[] items;
        
        
        public PlayerInfo() : base(MessageType.None)
        {

        }
        
        override public void decode(EndianBinaryReader binReader)
        {
        
            uid = binReader.ReadInt64();
            
            name = binReader.ReadUTF();
            
            status = binReader.ReadBoolean();
            
            type = (PlayerType)binReader.ReadInt32();
            
            maxResetTimes = binReader.ReadInt32();
            
            var len_items = binReader.ReadUInt16();
            items = new int[len_items];
            for (int i = 0; i < len_items; i++)
            {
                items[i] = binReader.ReadInt32();
            }
            
        }
        
        override public void encode(EndianBinaryWriter binWriter)
        {
        
            binWriter.Write(uid);
            
            binWriter.WriteUTF(name);
            
            binWriter.Write(status);
            
            binWriter.Write((int)type);
            
            binWriter.Write(maxResetTimes);
            
			ushort len_items = (ushort)items.Length;
			binWriter.Write(len_items);
            for (int i = 0; i < len_items; i++)
            {
                binWriter.Write(items[i]);
            }
            
        }
        
        public delegate void MessageHandler(PlayerInfo msg);
        static private List<MessageHandler> list_handler = new List<MessageHandler>();
        
        static public void RegisterHandler(MessageHandler handler)
        {
            list_handler.Add(handler);
        }

        static public void CallHandler(PlayerInfo msg)
        {
            int i = 0;
            while (i < list_handler.Count)
            {
                var handler = list_handler[i];
                //对象被回收了的  自动移除
                if (handler.Target == null || handler.Target.ToString() == "null")
                {
                    list_handler.RemoveAt(i);
                    continue;
                }
                try
                {
                    handler(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                i++;
            }
        }
    }

}
