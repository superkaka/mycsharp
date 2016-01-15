using KLib.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public class TestServer
    {

        //数据事件委托
        public delegate void DataReceivedHandler(byte[] bytes, Socket sender);
        //数据事件委托类型的事件
        public event DataReceivedHandler OnData;

        private Socket mainSocket = null;
        //监听进程
        private Thread thread_listen;
        //已连接客户端列表
        private Dictionary<Socket, ClientObject> dic_client = new Dictionary<Socket, ClientObject>();
        //是否已经在侦听状态
        private Boolean isListening = false;

        public const int packageHeadLen = 4;

        private void receive(Socket socket)
        {
            if (!checkSocketConnect(socket))
                return;

            var client = dic_client[socket];
            client.context_head.SetBuffer(new byte[packageHeadLen], 0, packageHeadLen);
            socket.ReceiveAsync(client.context_head);
        }

        private void OnHeadReceived(object sender, SocketAsyncEventArgs context_head)
        {
            var client = (ClientObject)context_head.UserToken;
            int len = BitConverter.ToInt32(context_head.Buffer, 0);
            len = NetUtils.ConvertToEndian(len, Endian.BigEndian);
            if (len > 0)
            {
                if (!checkSocketConnect(client.socket))
                    return;
                client.context_body.SetBuffer(new byte[len], 0, len);
                client.socket.ReceiveAsync(client.context_body);
            }
            else
            {
                receive((Socket)sender);
            }
        }

        private void OnBodyReceived(object sender, SocketAsyncEventArgs context_body)
        {
            var bytesReceived = context_body.Buffer;
            if (OnData != null)
                OnData(bytesReceived, ((ClientObject)context_body.UserToken).socket);

            receive((Socket)sender);
        }

        public void send(byte[] bytes, object client)
        {
            //写入4字节的包头长度
            var package = new byte[bytes.Length + packageHeadLen];
            Buffer.BlockCopy(BitConverter.GetBytes(NetUtils.ConvertToEndian(bytes.Length, Endian.BigEndian)), 0, package, 0, packageHeadLen);
            Buffer.BlockCopy(bytes, 0, package, packageHeadLen, bytes.Length);
            ((Socket)client).Send(package);
        }

        private bool checkSocketConnect(Socket socket)
        {
            if (!socket.Connected)
            {
                removeSocket(socket);
                return false;
            }
            return true;
        }

        private void removeSocket(Socket socket)
        {
            Console.WriteLine(String.Format("客户端{0}已断开", socket.RemoteEndPoint));
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            dic_client.Remove(socket);
        }

        private void listen()
        {
            while (true)
            {
                var clientSocket = mainSocket.Accept();
                var client = new ClientObject();
                client.context_head.Completed += OnHeadReceived;
                client.context_body.Completed += OnBodyReceived;
                client.socket = clientSocket;
                dic_client.Add(clientSocket, client);
                Console.WriteLine(String.Format("客户端{0}已连接", clientSocket.RemoteEndPoint));

                receive(clientSocket);
            }
        }

        public void startListen(int port)
        {
            if (isListening)
            {
                return;
            }

            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            mainSocket.Bind(serverEndPoint);
            mainSocket.Listen(100);
            thread_listen = new Thread(new ThreadStart(listen));
            thread_listen.Start();
            isListening = true;
        }

        public void stopListen()
        {
            if (!isListening)
            {
                return;
            }

            foreach (var client in dic_client.Keys)
            {
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch
                {
                }

            }

            try
            {
                thread_listen.Abort();
                mainSocket.Close();
            }
            catch
            {
            }

            isListening = false;
        }

        private class ClientObject
        {
            public Socket socket;
            public SocketAsyncEventArgs context_head = new SocketAsyncEventArgs();
            public SocketAsyncEventArgs context_body = new SocketAsyncEventArgs();
            public ClientObject()
            {
                context_head.UserToken = this;
                context_body.UserToken = this;
            }
        }

    }

}
