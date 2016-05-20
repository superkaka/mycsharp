using KLib.net.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    
    public class RPCClient
    {

        protected BaseConnection connection;
        protected PackageTranslator packager;

        public event CommonMessageHandler globalMessageHandler;
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

        public void RegisterMessageHandler(MessageType messageType, CommonMessageHandler handler)
        {

            ProtocolCenter.RegisterMessageHandler(messageType,handler);

        }

        public void Call(BaseProtocolVO vo)
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

            ProtocolCenter.DispatchMessage(vo);

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
