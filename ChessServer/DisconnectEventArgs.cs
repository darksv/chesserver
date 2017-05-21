using System;
using System.Net.Sockets;

namespace ChessServer
{
    public class DisconnectEventArgs : EventArgs
    {
        public Socket ClientSocket { get; set; }
    }
}