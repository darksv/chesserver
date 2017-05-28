using System;
using System.Net.Sockets;
using Chess.Common;

namespace Chess.Server
{
    public class Client
    {
        public PlayerStatus Status { get; set; } = PlayerStatus.Connected;
        public Socket Socket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public string Nick { get; set; } = string.Empty;
        public Game Game { get; set; }

        public Client(Socket socket)
        {
            Socket = socket;
        }
    }
}