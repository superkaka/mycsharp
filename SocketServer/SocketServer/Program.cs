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

            server = new RPCServer(new TestServer(), new PackageTranslator(new ProtocolCenter()));
            server.globalMessageHandler = globalHandler;
            server.RegisterMessageHandler(MessageType.RequestSendString, RequestSendString);
            server.RegisterMessageHandler(MessageType.RequestPlayers, RequestPlayers);
            server.StartListen(7666);

            Console.ReadLine();

        }

        static void globalHandler(BaseProtocolVO baseVO, object client)
        {
            Console.WriteLine(String.Format("收到协议:{0}", baseVO.MessageType));

            //发回所有收到的消息
            //server.Send(baseVO, client);
        }

        static void RequestSendString(BaseProtocolVO baseVO, object client)
        {
            var vo = (RequestSendString)baseVO;
            var response = ResponseSendString.CreateInstance();
            response.content = vo.content;
            server.Send(response, client);
        }

        static void RequestPlayers(BaseProtocolVO baseVO, object client)
        {
            var vo = (RequestPlayers)baseVO;

            var players = new PlayerInfo[3];
            for (int i = 1; i <= 3; i++)
            {
                players[i - 1] = new PlayerInfo()
                {
                    uid = i + 1000,
                    name = "qqq" + i,
                    status = i % 2 == 0,
                    type = (PlayerType)i,
                    maxResetTimes = i,
                    items = new int[] { 111, 222, 99999 },
                };
            }
            var response = new ResponsePlayers()
            {
                status = true,
                players = players,
            };
            server.Send(response, client);
        }

    }
}
