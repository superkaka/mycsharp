using KLib.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KLib.net.connection
{
    public class SocketConnection : BaseConnection
    {

        public const int packageHeadLen = 4;

        private Socket socket;
        private Thread thread_receive;
        private string host;
        private int port;

        public SocketConnection(string host, int port)
        {
            this.host = host;
            this.port = port;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        override public void Connect()
        {

            if (Connected)
            {
                throw new Exception("尝试对处于连接状态的socket进行连接操作");
            }

            var args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnectCompleted);
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            socket.ConnectAsync(args);

        }

        private void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {

            if (e.SocketError == SocketError.Success)
            {
                thread_receive = new Thread(new ThreadStart(receive));
                thread_receive.IsBackground = true;
                thread_receive.Start();

                ConnectSucess();
            }
            else
            {
                ConnectFailed(e.SocketError.ToString());
            }

        }


        public bool Connected { get { return socket.Connected; } }

        override protected void SendData(byte[] bytes)
        {

            //写入4字节的包头长度
            var package = new byte[bytes.Length + packageHeadLen];
            Buffer.BlockCopy(BitConverter.GetBytes(NetUtils.ConvertToEndian(bytes.Length, Endian.BigEndian)), 0, package, 0, packageHeadLen);
            Buffer.BlockCopy(bytes, 0, package, packageHeadLen, bytes.Length);

            var args = new SocketAsyncEventArgs();
            args.Completed += OnSendCompleted;
            args.SetBuffer(package, 0, package.Length);
            socket.SendAsync(args);

        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {

        }

        private byte[] headBytes = new byte[packageHeadLen];
        private void receive()
        {
            while (true)
            {

                byte[] receiveBytes;

                try
                {
                    if (!Connected)
                    {
                        return;
                    }

                    socket.Receive(headBytes, packageHeadLen, SocketFlags.None);
                    int len = BitConverter.ToInt32(headBytes, 0);
                    len = NetUtils.ConvertToEndian(len, Endian.BigEndian);
                    receiveBytes = new byte[len];
                    socket.Receive(receiveBytes, len, SocketFlags.None);
                }
                catch (Exception e)
                {
                    //throw e;
                    ConnectClose(e.ToString());
                    break;
                }

                DistributeData(receiveBytes);

            }
        }

    }
}
