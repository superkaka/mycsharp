using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLib.net.connection;
using protocol;
using protocol.vo;

namespace TestProtocol
{
    public class TestProtocol
    {
        SocketConnection connection;
        PackageTranslator translator;
        RPCClient client;
        public void start()
        {

            translator = MessageRegister.GetTranslator();
            connection = new SocketConnection("127.0.0.1", 7666);
            client = new RPCClient(connection, translator);
            client.RegisterMessageHandler(Protocol.testCommonStruct, testCommonStruct);
            client.RegisterMessageHandler(Protocol.sendString, sendString);
            client.globalMessageHandler += globalMessageHandler;

            client.OnConnectSuccess += OnConnectSuccess;
            client.OnConnectFail += OnConnectFail;
            client.OnConnectClose += OnConnectClose;
            client.Connect();

            /*
            connection.OnConnectSuccess += OnConnectSuccess;
            connection.OnData += OnData;
            connection.OnConnectClose += OnConnectClose;
            connection.Connect();
            */

            while (true)
            {
                var input = Console.ReadLine();
                var sendStr = new sendStringVO();
                sendStr.str = input;
                client.Call(sendStr);
            }


        }

        void OnConnectSuccess()
        {
            sendtestCommonStructVO();
            sendsendLoginVO();
        }

        void OnConnectFail(string msg)
        {
            Console.WriteLine("连接失败");
            Console.WriteLine(msg);
        }

        void OnConnectClose(string msg)
        {
            Console.WriteLine("连接已关闭");
            Console.WriteLine(msg);
        }

        void sendtestCommonStructVO()
        {
            var vo = new testCommonStructVO();
            vo.battleId = 12593;
            vo.damageList = new CommonStructVO[]{
            new CommonStructVO()
            {
                m_string="CommonStructVO",
                m_Boolean=true,
                m_int=-325,
                m_Binary=new byte[]{255,0}
            },
             new CommonStructVO()
            {
                m_string="CommonStructVO2",
                m_Boolean=false,
                m_int=325,
                m_Binary=new byte[]{255,0}
            },
             new CommonStructVO()
            {
                m_string="CommonStructVO3",
                m_Boolean=true,
                m_int=999,
                m_Binary=new byte[]{255,0}
            }
            };
            var bytes = translator.Encode(vo);
            connection.Send(bytes);
        }

        void sendsendLoginVO()
        {
            var vo = new sendLoginVO()
            {
                actorId = 7999,
                m_byte = -9,
                m_date = new DateTime(1987, 7, 19, 23, 16, 59, DateTimeKind.Local),
                m_float = -96666366.978945,
                testStruct = new sendLogin_testStructVO()
                {
                    m_Binary = new byte[] { 77, 88 },
                    m_Boolean = true,
                    m_int = 763,
                    m_string = @"战斗结束 调用 
http://192.168.10.10/battle/end?BSID=057b02d9c00fa3d883b2edc85d516903&battle_level_id=101000&difficulty=normal&battle_result={}",
                    m_uint = 111555888,
                    m_ushort = 65436,
                    m_long = long.MaxValue,
                    m_ulong = ulong.MaxValue,
                },
                playerList = new sendLogin_playerListVO[]
                {
                new sendLogin_playerListVO(){actorId=1,teamId=666},
                new sendLogin_playerListVO(){actorId=2,teamId=667},
                new sendLogin_playerListVO(){actorId=2,teamId=667},
                new sendLogin_playerListVO(){actorId=2,teamId=667},
                }
            };
            var bytes = translator.Encode(vo);
            connection.Send(bytes);
        }

        private void sendString(BaseVO baseVO)
        {
            var vo = (sendStringVO)baseVO;
            Console.WriteLine("receive from server:");
            Console.WriteLine(vo.str);
        }

        private void testCommonStruct(BaseVO baseVO)
        {

        }

        private void globalMessageHandler(BaseVO baseVO)
        {

        }

        void OnData(byte[] bytes)
        {
            var vo = translator.Decode(bytes);
        }

    }
}
