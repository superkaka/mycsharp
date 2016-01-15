using KLib.net.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    public delegate void MessageHandler(BaseVO vo);
    public class RPCClient
    {

        protected BaseConnection connection;
        protected PackageTranslator packager;

        protected Dictionary<int, MessageHandler> dic_handler = new Dictionary<int, MessageHandler>();
        public event MessageHandler globalMessageHandler;
        public event Action OnConnectSuccess;
        public event ConnectionEventHandler OnConnectFail;
        public event ConnectionEventHandler OnConnectClose;

        public RPCClient(BaseConnection connection, PackageTranslator packager)
        {
            this.connection = connection;
            this.packager = packager;
        }

        public void Connect()
        {
            connection.OnData += connectDataHandler;
            connection.OnConnectSuccess += connectionSuccessHandler;
            connection.OnConnectFail += connectionFailHandler;
            connection.OnConnectClose += connectionCloseHandler;

            this.connection.Connect();

        }

        public void RegisterMessageHandler(int procedureId, MessageHandler handler)
        {

            dic_handler[procedureId] = handler;

        }

        public void Call(BaseVO vo)
        {

            var bytes = packager.Encode(vo);
            send(bytes);

        }

        protected void send(byte[] data)
        {
            connection.Send(data);
        }

        protected void connectDataHandler(byte[] bytes)
        {

            var vo = packager.Decode(bytes);
            if (null != globalMessageHandler)
                globalMessageHandler(vo);

            MessageHandler handler;
            if (dic_handler.TryGetValue(vo.ProtocolId, out handler))
            {
                handler(vo);
            }
            else
            {
                Console.WriteLine("过程" + vo.ProtocolId + "缺少处理函数！");
            }

        }

        private void connectionSuccessHandler()
        {
            if (OnConnectSuccess != null)
                OnConnectSuccess();
        }
        private void connectionFailHandler(string msg)
        {
            if (OnConnectFail != null)
                OnConnectFail(msg);
        }

        private void connectionCloseHandler(string msg)
        {
            if (OnConnectClose != null)
                OnConnectClose(msg);
        }

    }
}
