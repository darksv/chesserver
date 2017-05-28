using System;

namespace Chess.Server
{
    public static class ChessServerApp
    {
        private static void Log(object value)
        {
            Console.WriteLine($"[{DateTime.Now}] {value}");
        }

        public static int Main(string[] args)
        {
            const ushort port = 11000;

            Log($"Starting server :{port}...");

            var server = new ChessServer(Log);
            server.Run(port);

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
            return 0;
        }
    }
}