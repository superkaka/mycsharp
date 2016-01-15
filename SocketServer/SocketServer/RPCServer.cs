using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public delegate void MessageHandler(BaseVO vo,object client);
    public class RPCServer
    {

        private TestServer server;
        private PackageTranslator translator;

        protected Dictionary<int, MessageHandler> dic_handler = new Dictionary<int, MessageHandler>();
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

        public void Send(BaseVO vo, object client)
        {
            var bytes = translator.Encode(vo);
            server.send(bytes, client);
        }

        public void RegisterMessageHandler(int procedureId, MessageHandler handler)
        {

            dic_handler[procedureId] = handler;

        }

        private void OnData(byte[] bytes, object sender)
        {

            var vo = translator.Decode(bytes);
            if (null != globalMessageHandler)
                globalMessageHandler(vo,sender);

            MessageHandler handler;
            if (dic_handler.TryGetValue(vo.ProtocolId, out handler))
            {
                handler(vo, sender);
            }
            else
            {
                Console.WriteLine("过程" + vo.ProtocolId + "缺少处理函数！");
            }

        }

    }
}
