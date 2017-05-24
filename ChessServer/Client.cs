using System;
using System.Net.Sockets;

namespace ChessServer
{
    public class Client
    {
        public ClientStatus Status { get; set; } = ClientStatus.Connected;
        public Socket Socket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public string Nick { get; set; } = string.Empty;

        public Client(Socket socket)
        {
            Socket = socket;
        }
    }
}