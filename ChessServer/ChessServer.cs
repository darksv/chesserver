﻿using System;
using System.Collections.Generic;
using System.Linq;

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

            var player = new Player
            {
                Id = Guid.NewGuid(),
                Nick = "Anon",
                ClientSocket = args.ClientSocket
            };

            lock (_lock)
            {
                _players.Add(player);
            }
        }

        private void ServerOnReceive(object sender, ReceiveEventArgs args)
        {
            var s = (Server) sender;

            Player currentPlayer;
            IEnumerable<Player> otherPlayers;
            lock (_players)
            {
                currentPlayer = _players.First(p => p.ClientSocket == args.ClientSocket);
                otherPlayers = _players.Where(p => p != currentPlayer).ToArray();
            }

            if (args.Message.StartsWith("/nick"))
            {
                currentPlayer.Nick = args.Message.Substring(5).Trim();
            }
            else
            {
                Console.WriteLine($"{DateTime.Now} {currentPlayer.Nick} [{args.ClientSocket.RemoteEndPoint}] {args.Message}");
                foreach (var player in otherPlayers)
                {
                    s.Send(player.ClientSocket, currentPlayer.Nick + ": " + args.Message + "\n");
                }
            }
        }
    }
}