using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace ChessServer
{
    public enum ClientStatus
    {
        Connected,
        Joined,
        OnGame,
        Disconnected
    }

    public class Client
    {
        public ClientStatus Status { get; set; } = ClientStatus.Connected;
        public Socket Socket { get; }
        public Guid Id { get; } = Guid.NewGuid();
        public string Nick { get; set; } = string.Empty;

        public Client(Socket socket)
        {
            Socket = socket;
        }
    }

    public class ChessServer
    {
        private readonly Server _server = new Server();
        private readonly object _lock = new object();
        private readonly List<Client> _clients = new List<Client>();
        private readonly Dictionary<string, Action<Action<string>, Client, string[]>> _handlers;

        public ChessServer()
        {
            _handlers = new Dictionary<string, Action<Action<string>, Client, string[]>>
            {
                {"join", HandleJoin},
                {"ping", HandlePing},
                {"invite", HandleInvite}
            };
        }

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
                var client = _clients.FirstOrDefault(p => p.Socket == args.ClientSocket);
                if (client != null)
                {
                    client.Status = ClientStatus.Disconnected;
                }
            }

            Console.WriteLine($"{DateTime.Now} We've lost connection with {args.ClientSocket.RemoteEndPoint}");
        }

        private void ServerOnConnect(object sender, ConnectEventArgs args)
        {
            var client = new Client(args.ClientSocket);

            lock (_lock)
            {
                _clients.Add(client);
            }

            Console.WriteLine($"{DateTime.Now} We've established connection with {args.ClientSocket.RemoteEndPoint}");
        }

        private void ServerOnReceive(object sender, ReceiveEventArgs args)
        {
            var s = (Server) sender;

            var client = FindClientBySocket(args.ClientSocket);

            var @params = args.Message.Split(';');
            if (_handlers.TryGetValue(@params[0], out var handler))
            {
                handler(x => s.Send(args.ClientSocket, x), client, @params);
            }
            else
            {
                Console.WriteLine("Error: unknown command");
            }
        }

        private void HandleJoin(Action<string> send, Client client, string[] args)
        {
            if (client.Status != ClientStatus.Connected)
            {
                send("join;1;already_joined\n");
                return;
            }
            var nick = args[1];
            if (IsNickTaken(nick))
            {
                send("join;2;nick_taken\n");
                return;
            }

            client.Nick = nick;
            client.Status = ClientStatus.Joined;
            send($"join;0;{client.Id}\n");

            Client[] clients;
            lock (_lock)
            {
                clients = _clients.Where(c => c != client && c.Status == ClientStatus.Joined).ToArray();
            }

            send($"clients;{string.Join(",", clients.Select(c => c.Nick))}\n");
        }

        private void HandlePing(Action<string> send, Client client, string[] args)
        {
            send("pong\n");
        }

        private void HandleInvite(Action<string> send, Client client, string[] args)
        {
            
        }

        private bool IsNickTaken(string nick)
        {
            lock (_lock)
            {
                return _clients.Any(p => p.Nick == nick);
            }
        }
 
        private Client FindClientBySocket(Socket socket)
        {
            lock (_lock)
            {
                return _clients.FirstOrDefault(p => p.Socket == socket);
            }
        }
    }
}