using KLib.net.connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    public delegate void MessageHandler(BaseProtocolVO vo);
    public class RPCClient
    {

        protected BaseConnection connection;
        protected PackageTranslator packager;

        protected Dictionary<int, List<MessageHandler>> dic_handler = new Dictionary<int, List<MessageHandler>>();
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

            List<MessageHandler> list_handler;
            if (dic_handler.TryGetValue(procedureId, out list_handler) == false)
            {
                list_handler = dic_handler[procedureId] = new List<MessageHandler>();
            }
            list_handler.Add(handler);

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

            List<MessageHandler> list_handler;
            if (dic_handler.TryGetValue(vo.ProtocolId, out list_handler))
            {
                int i = 0;
                while (i < list_handler.Count)
                {
                    var handler = list_handler[i];
                    //对象被回收了的  自动移除
                    if (handler.Target == null || handler.Target.ToString() == "null")
                    {
                        list_handler.RemoveAt(i);
                        continue;
                    }
                    try
                    {
                        handler(vo);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    i++;
                }
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
