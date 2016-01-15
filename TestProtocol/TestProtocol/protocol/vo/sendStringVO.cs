
using System;
using System.IO;
using System.Text;
using KLib.utils;

namespace protocol.vo
{
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
