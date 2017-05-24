using System;
using System.Net.Sockets;

namespace ChessServer
{
    public class ConnectionEventArgs : EventArgs
    {
        public Socket ClientSocket { get; }

        public ConnectionEventArgs(Socket socket)
        {
            ClientSocket = socket;
        }
    }
}
