using System.Net.Sockets;
using System.Text;

namespace ChessServer
{
    public class ConnectionState
    {
        public Socket Socket;
        public const int BufferSize = 1024;
        public readonly byte[] Buffer = new byte[BufferSize];
        public readonly StringBuilder StringBuilder = new StringBuilder();
    }
}