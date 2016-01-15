
using System;
using System.Text;
using protocol;
using protocol.vo;

namespace protocol
{
    public class MessageRegister
    {
        
        static public PackageTranslator GetTranslator()
        {
            var translator = new PackageTranslator();
            Register(translator);
            return translator;
        }
        
        static public void Register(PackageTranslator target)
        {
            
            target.RegisterMessage(Protocol.sendLogin, Create_sendLoginVO);
        
            target.RegisterMessage(Protocol.testCommonStruct, Create_testCommonStructVO);
        
            target.RegisterMessage(Protocol.sendString, Create_sendStringVO);
        
        }
        
        
        static public BaseVO Create_sendLoginVO()
        {
            return new sendLoginVO();
        }
        
        static public BaseVO Create_testCommonStructVO()
        {
            return new testCommonStructVO();
        }
        
        static public BaseVO Create_sendStringVO()
        {
            return new sendStringVO();
        }
        
        
    }
}
