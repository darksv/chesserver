using System;
using System.Net.Sockets;

namespace ChessServer
{
    public class ReceiveEventArgs : EventArgs
    {
        public Socket ClientSocket { get; set; }
        public string Message { get; set; }
    }
}
