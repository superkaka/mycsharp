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
    /// 
    /// </summary>
    public class RequestSendString : BaseProtocolVOGeneric<RequestSendString>
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        public string content;
        
        
        public RequestSendString() : base(MessageType.RequestSendString)
        {

        }
        
        override public void decode(ProtocolBinaryReader binReader)
        {
        
            content = binReader.ReadString();
            
        }
        
        override public void encode(ProtocolBinaryWriter binWriter)
        {
        
            binWriter.Write(content);
            
        }
        
    }

}
