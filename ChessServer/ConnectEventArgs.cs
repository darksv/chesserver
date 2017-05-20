using System;
using System.Net.Sockets;

namespace ChessServer
{
    public class ConnectEventArgs : EventArgs
    {
        public Socket ClientSocket { get; set; }
    }
}
