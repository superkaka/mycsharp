using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using protocol;
using protocol.vo;
using System.Net.Sockets;

namespace SocketServer
{
    class Program
    {
        static RPCServer server;
        static void Main(string[] args)
        {

            server = new RPCServer(new TestServer(), MessageRegister.GetTranslator());
            server.globalMessageHandler = globalHandler;
            server.RegisterMessageHandler(Protocol.sendString, sendStringHandler);
            server.StartListen(7666);

            Console.ReadLine();

        }

        static void globalHandler(BaseVO baseVO, object client)
        {
            Console.WriteLine(String.Format("收到协议:{0}", baseVO.ProtocolId));

            //发回所有收到的消息
            server.Send(baseVO, client);
        }

        static void sendStringHandler(BaseVO baseVO, object client)
        {
            var vo = (sendStringVO)baseVO;
            server.Send(vo, client);
        }

    }
}
