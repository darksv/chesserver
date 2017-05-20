using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChessServer
{
    public class ConnectionState
    {
        public Socket socket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder stringBuilder = new StringBuilder();
    }

    //    private async Task HandleConnectionAsync(TcpClient tcpClient)
    //    {
    //    await Task.Yield();
    //
    //    var player = new Player
    //        {
    //            Id = Guid.NewGuid(),
    //            Nick = "alojzy"
    //        };
    //
    //    using (var networkStream = tcpClient.GetStream())
    //    {
    //        while (true)
    //        {
    //            await WriteAsync(networkStream, ">> ");
    //
    //            var request = await ReadAsync(networkStream, 100);
    //            var response = JsonConvert.SerializeObject(player, Formatting.None) + "\n";
    //            await WriteAsync(networkStream, response);
    //        }
    //    }
    //    }

    public class Server
    {
        public void Start()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, 11000);
            var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(int.MaxValue);

                while (true)
                {
                    var clientSocket = listener.Accept();

                    Connect?.Invoke(this, new ConnectEventArgs { ClientSocket = clientSocket });

                    var state = new ConnectionState { socket = clientSocket };
                    clientSocket.BeginReceive(state.buffer, 0, ConnectionState.BufferSize, 0, ReadCallback, state);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            var connectionState = (ConnectionState)ar.AsyncState;
            var clientSocket = connectionState.socket;

            int bytesRead = clientSocket.EndReceive(ar);
            if (bytesRead <= 0)
            {
                return;
            }

            var sb = connectionState.stringBuilder;
            sb.Append(Encoding.ASCII.GetString(connectionState.buffer, 0, bytesRead));

            var parts = sb.ToString().Split('\n');

            sb.Clear();
            sb.Append(parts.Last());

            foreach (var part in parts.Take(parts.Length - 1))
            {
                Receive?.Invoke(this, new ReceiveEventArgs
                {
                    ClientSocket = connectionState.socket,
                    Message = part
                });
            }

            clientSocket.BeginReceive(connectionState.buffer, 0, ConnectionState.BufferSize, 0, ReadCallback, connectionState);
        }

        public void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                SendCallback, handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                var clientSocket = (Socket)ar.AsyncState;

                int bytesSent = clientSocket.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public event EventHandler<ReceiveEventArgs> Receive;
        public event EventHandler<ConnectEventArgs> Connect;
    }

    public class ChessServer
    {
        private static readonly Server _server = new Server();
        public static int Main(string[] args)
        {
            _server.Receive += _server_Receive;
            _server.Connect += ServerOnConnect;
            _server.Start();


            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
            return 0;
        }

        private static void ServerOnConnect(object sender, ConnectEventArgs args)
        {
            Console.WriteLine($"{DateTime.Now} We've got some client! Wow - {args.ClientSocket.RemoteEndPoint}");
        }

        private static void _server_Receive(object sender, ReceiveEventArgs args)
        {
            var s = (Server) sender;

            Console.WriteLine($"{DateTime.Now} {args.ClientSocket.RemoteEndPoint} {args.Message}");
            s.Send(args.ClientSocket, "OK\n");
        }
    }
}