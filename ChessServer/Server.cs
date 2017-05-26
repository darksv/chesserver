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

            Socket clientSocket = null;
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    clientSocket = listener.Accept();

                    Connect?.Invoke(this, new ConnectionEventArgs(clientSocket));

                    var state = new ConnectionState { Socket = clientSocket };
                    clientSocket.BeginReceive(state.Buffer, 0, ConnectionState.BufferSize, 0, ReadCallback, state);
                }
            }
            catch (Exception e)
            {
                HandleError(clientSocket, e);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var connectionState = (ConnectionState)ar.AsyncState;
            var clientSocket = connectionState.Socket;

            try
            {
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
            catch (Exception e)
            {
                HandleError(clientSocket, e);
            }
        }

        public void Send(Socket clientSocket, string data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                clientSocket.BeginSend(byteData, 0, byteData.Length, 0,
                    SendCallback, clientSocket);
            }
            catch (Exception e)
            {
                HandleError(clientSocket, e);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            var clientSocket = (Socket)result.AsyncState;
            try
            {
                clientSocket.EndSend(result);
            }
            catch (Exception e)
            {
                HandleError(clientSocket, e);
            }
        }

        private void HandleError(Socket clientSocket, Exception e = null)
        {
            if (clientSocket == null)
            {
                return;
            }

            var disconnectionReason = e?.Message ?? string.Empty;
            Disconnect?.Invoke(this, new ConnectionEventArgs(clientSocket, disconnectionReason));
        }

        public event EventHandler<ReceiveEventArgs> Receive;
        public event EventHandler<ConnectionEventArgs> Connect;
        public event EventHandler<ConnectionEventArgs> Disconnect;
    }
}