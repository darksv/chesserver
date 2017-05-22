using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ChessServer
{
    public class ChessServer
    {
        private readonly Server _server = new Server();
        private readonly object _lock = new object();
        private readonly List<Player> _players = new List<Player>();

        public void Run()
        {
            _server.Receive += ServerOnReceive;
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.Start();
        }

        private void ServerOnDisconnect(object sender, DisconnectEventArgs args)
        {
            lock (_lock)
            {
                var player = _players.First(p => p.ClientSocket == args.ClientSocket);
                _players.Remove(player);
            }

            Console.WriteLine($"{DateTime.Now} We've lost connection with {args.ClientSocket.RemoteEndPoint}");
        }

        private void ServerOnConnect(object sender, ConnectEventArgs args)
        {
            Console.WriteLine($"{DateTime.Now} We've established connection with {args.ClientSocket.RemoteEndPoint}");
        }

        private void ServerOnReceive(object sender, ReceiveEventArgs args)
        {
            var s = (Server) sender;
            
            var @params = args.Message.Split(';');
            switch (@params[0])
            {
                case "join":
                    var player = FindPlayerBySocket(args.ClientSocket);
                    if (player != null)
                    {
                        s.Send(args.ClientSocket, "join;1;already_joined\n");
                        return;
                    }
                    var nick = @params[1];
                    if (IsNickTaken(nick))
                    {
                        s.Send(args.ClientSocket, "join;2;nick_taken\n");
                        return;
                    }
                    
                    player = new Player
                    {
                        Id = Guid.NewGuid(),
                        Nick = nick,
                        ClientSocket = args.ClientSocket
                    };

                    lock (_lock)
                    {
                        _players.Add(player);
                    }

                    s.Send(args.ClientSocket, $"join;0;{player.Id}\n");

                    break;
                case "ping":
                    s.Send(args.ClientSocket, "pong\n");
                    break;
                default:
                    Console.WriteLine("Error: unknown command");
                    break;
            }
        }

        private bool IsNickTaken(string nick)
        {
            lock (_lock)
            {
                return _players.Any(p => p.Nick == nick);
            }
        }

        private Player FindPlayerBySocket(Socket socket)
        {
            lock (_lock)
            {
                return _players.FirstOrDefault(p => p.ClientSocket == socket);
            }
        }
    }
}