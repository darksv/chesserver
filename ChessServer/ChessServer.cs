using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;

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
        private readonly Dictionary<string, Action<Server, Client, string>> _handlers;

        public ChessServer()
        {
            _handlers = new Dictionary<string, Action<Server, Client, string>>
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

            Client client;
            lock (_lock)
            {
                client = _clients.FirstOrDefault(p => p.Socket == args.ClientSocket);
            }

            // TODO: handle illformed message
            var message = JsonConvert.DeserializeObject<Message>(args.Message);
            var messageType = message?.Type ?? string.Empty;
            if (_handlers.TryGetValue(messageType, out var handler))
            {
                handler(s, client, args.Message);
            }
            else
            {
                Console.WriteLine("Error: unknown command");
            }
        }

        private void HandleJoin(Server server, Client client, string data)
        {
            if (client.Status != ClientStatus.Connected)
            {
                server.Send(client.Socket, MakeResponse(new JoinResponse(JoinStatus.AlreadyJoined)));
                return;
            }

            var nick = JsonConvert.DeserializeObject<JoinRequest>(data).Nick;
            if (IsNickTaken(nick))
            {
                server.Send(client.Socket, MakeResponse(new JoinResponse(JoinStatus.NickExists)));
                return;
            }

            client.Nick = nick;
            client.Status = ClientStatus.Joined;
            server.Send(client.Socket, MakeResponse(new JoinResponse(JoinStatus.Success)));

            Client[] clients;
            lock (_lock)
            {
                clients = _clients.Where(c => c != client && c.Status == ClientStatus.Joined).ToArray();
            }

            server.Send(client.Socket, MakeResponse(new OnlinePlayers
            {
                Players = clients
                    .Select(c => new PlayerItem
                    {
                        Id = c.Id,
                        Nick = c.Nick,
                        Status = c.Status
                    })
                    .ToArray()
            }));
        }

        private string MakeJson(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.None);
        }

        private string MakeResponse<T>(T value) 
            where T : Message
        {
            if (string.IsNullOrEmpty(value.Type))
            {
                var messageTypeAttribute = typeof(T)
                    .GetCustomAttributes(false)
                    .OfType<MessageTypeAttribute>()
                    .FirstOrDefault();
                value.Type = messageTypeAttribute?.Type ?? string.Empty;
            }
            
            return $"{MakeJson(value)}\n";
        }

        private void HandlePing(Server server, Client client, string data)
        {
            server.Send(client.Socket, MakeResponse(new PongResponse()));
        }

        private void HandleInvite(Server server, Client client, string data)
        {
            var clientId = JsonConvert.DeserializeObject<InviteSendRequest>(data).Id;
            if (clientId == client.Id)
            {
                server.Send(client.Socket, MakeResponse(new InviteSendResponse(InviteStatus.SelfInvite)));
                return;
            }

            Client invitedClient;
            lock (_lock)
            {
                invitedClient = _clients.FirstOrDefault(c => c.Id == clientId);
            }

            if (invitedClient == null)
            {
                server.Send(client.Socket, MakeResponse(new InviteSendResponse(InviteStatus.PlayerNotExist)));
                return;
            }

            server.Send(client.Socket, MakeResponse(new InviteSendResponse(InviteStatus.Success)));
            server.Send(invitedClient.Socket,
                MakeResponse(new InviteReceiveRequest
                {
                    Player = new PlayerItem {Id = client.Id, Nick = client.Nick, Status = client.Status}
                }));
        }

        private bool IsNickTaken(string nick)
        {
            lock (_lock)
            {
                return _clients.Any(p => p.Nick == nick);
            }
        }
    }
}