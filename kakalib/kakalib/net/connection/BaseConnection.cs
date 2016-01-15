using KLib.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLib.net.connection
{
    public delegate void ConnectionEventHandler(string msg);
    public abstract class BaseConnection
    {

        public delegate void BytesDelegate(byte[] bytes);
        public event BytesDelegate OnData;
        public event Action OnConnectSuccess;
        public event ConnectionEventHandler OnConnectFail;
        public event ConnectionEventHandler OnConnectClose;

        //待发送的数据包队列
        protected Queue<byte[]> queue_send = new Queue<byte[]>();
        //已收到未处理的数据包队列
        protected Queue<byte[]> queue_receive = new Queue<byte[]>();

        private bool blockWrite;

        public bool BlockWrite
        {
            get { return blockWrite; }
            set
            {
                blockWrite = value;
                if (blockWrite == false) Flush();
            }
        }
        private bool blockRead;

        public bool BlockRead
        {
            get { return blockRead; }
            set
            {
                blockRead = value;
                if (blockRead == false)
                {
                    while (queue_receive.Count > 0)
                    {
                        DistributeData(queue_receive.Dequeue());
                    }

                }

            }
        }

        public void Send(byte[] bytes)
        {
            if (blockWrite)
                queue_send.Enqueue(bytes);
            else
                SendData(bytes);
        }

        //将队列中累积的数据全部发送
        private void Flush()
        {
            while (queue_send.Count > 0)
            {
                SendData(queue_send.Dequeue());
            }
        }

        public virtual void Connect()
        {

        }

        protected virtual void SendData(byte[] bytes)
        {
            //子类覆写
        }

        protected void DistributeData(byte[] bytes)
        {
            if (blockRead)
                queue_receive.Enqueue(bytes);
            else
            {
                if (OnData != null)
                    OnData(bytes);
            }

        }

        protected virtual void ConnectSucess()
        {
            if (OnConnectSuccess != null)
                OnConnectSuccess();
        }

        protected virtual void ConnectFailed(string msg)
        {
            if (OnConnectFail != null)
                OnConnectFail(msg);
        }

        protected virtual void ConnectClose(string msg)
        {
            if (OnConnectClose != null)
                OnConnectClose(msg);
        }

    }
}
