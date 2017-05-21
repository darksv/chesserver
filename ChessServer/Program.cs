using System;

namespace ChessServer
{
    public class ChessServerApp
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("Starting server...");

            var server = new ChessServer();
            server.Run();

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
            return 0;
        }
    }
}