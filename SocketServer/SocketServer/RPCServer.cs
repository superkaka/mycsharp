using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public delegate void MessageHandler(BaseProtocolVO vo, object client);
    public class RPCServer
    {

        private TestServer server;
        private PackageTranslator translator;

        protected Dictionary<MessageType, MessageHandler> dic_handler = new Dictionary<MessageType, MessageHandler>();
        public MessageHandler globalMessageHandler;
        public RPCServer(TestServer server, PackageTranslator translator)
        {
            this.server = server;
            this.translator = translator;
            server.OnData += OnData;
        }

        public void StartListen(int port)
        {
            server.startListen(port);

        }

        public void Send(BaseProtocolVO vo, object client)
        {
            var bytes = translator.Encode(vo);
            server.send(bytes, client);
        }

        public void RegisterMessageHandler(MessageType procedureType, MessageHandler handler)
        {

            dic_handler[procedureType] = handler;

        }

        private void OnData(byte[] bytes, object sender)
        {

            var vo = translator.Decode(bytes);
            if (null != globalMessageHandler)
                globalMessageHandler(vo, sender);

            MessageHandler handler;
            if (dic_handler.TryGetValue(vo.MessageType, out handler))
            {
                handler(vo, sender);
            }
            else
            {
                Console.WriteLine("过程" + vo.MessageType + "缺少处理函数！");
            }

        }

    }
}
