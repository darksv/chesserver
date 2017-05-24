using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChessServer
{
    public class Server
    {
        public void Start(ushort port)
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, port);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    var clientSocket = listener.Accept();

                    Connect?.Invoke(this, new ConnectionEventArgs(clientSocket));

                    var state = new ConnectionState { Socket = clientSocket };
                    clientSocket.BeginReceive(state.Buffer, 0, ConnectionState.BufferSize, 0, ReadCallback, state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var connectionState = (ConnectionState)ar.AsyncState;
            var clientSocket = connectionState.Socket;

            int bytesRead = clientSocket.EndReceive(ar);
            if (bytesRead <= 0)
            {
                return;
            }

            var sb = connectionState.StringBuilder;
            sb.Append(Encoding.ASCII.GetString(connectionState.Buffer, 0, bytesRead));

            var parts = sb.ToString().Split('\n');

            sb.Clear();
            sb.Append(parts.Last());

            foreach (var part in parts.Take(parts.Length - 1))
            {
                Receive?.Invoke(this, new ReceiveEventArgs
                {
                    ClientSocket = connectionState.Socket,
                    Message = part
                });
            }

            clientSocket.BeginReceive(connectionState.Buffer, 0, ConnectionState.BufferSize, 0, ReadCallback, connectionState);
        }

        public void Send(Socket handler, string data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    SendCallback, handler);
            }
            catch (SocketException e)
            {
                Disconnect?.Invoke(this, new ConnectionEventArgs(handler));

                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var clientSocket = (Socket) ar.AsyncState;
                clientSocket.EndSend(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().FullName}: {e.Message}");
            }
        }

        public event EventHandler<ReceiveEventArgs> Receive;
        public event EventHandler<ConnectionEventArgs> Connect;
        public event EventHandler<ConnectionEventArgs> Disconnect;
    }
}