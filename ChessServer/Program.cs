using System;

namespace ChessServer
{
    public class ChessServerApp
    {
        private static void Log(object value)
        {
            Console.WriteLine($"[{DateTime.Now}] {value}");
        }

        public static int Main(string[] args)
        {
            Log("Starting server...");

            var server = new ChessServer(Log);
            server.Run();

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
            return 0;
        }
    }
}