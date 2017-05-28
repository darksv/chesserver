using System;
using System.Net.Sockets;

namespace Chess.Server
{
    public class ConnectionEventArgs : EventArgs
    {
        public Socket ClientSocket { get; }
        public string DisconnectionReason { get; }

        public ConnectionEventArgs(Socket socket, string disconnectionReason = null)
        {
            ClientSocket = socket;
            DisconnectionReason = disconnectionReason;
        }
    }
}
