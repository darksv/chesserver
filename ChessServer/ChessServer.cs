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
        private readonly Dictionary<string, Action<Client, string>> _handlers;

        public ChessServer()
        {
            _handlers = new Dictionary<string, Action<Client, string>>
            {
                {"join", HandleJoin},
                {"ping", HandlePing},
                {"send_invite", HandleSendInvite},
                {"answer_invite", HandleAnswerInvite},
                {"get_players", HandleGetPlayers}
            };
        }
        
        public void Run()
        {
            _server.Receive += ServerOnReceive;
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.Start();
        }

        private void ServerOnDisconnect(object sender, ConnectionEventArgs args)
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

        private void ServerOnConnect(object sender, ConnectionEventArgs args)
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
            Client client;
            lock (_lock)
            {
                client = _clients.FirstOrDefault(p => p.Socket == args.ClientSocket);
            }

            Message message;
            try
            {
                message = JsonConvert.DeserializeObject<Message>(args.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Error: invalid message format");
                return;
            }

            var messageType = message?.Type ?? string.Empty;
            if (_handlers.TryGetValue(messageType, out var handler))
            {
                handler(client, args.Message);
            }
            else
            {
                Console.WriteLine("Error: unknown command");
            }
        }

        #region Message Handlers

        private void HandlePing(Client client, string data)
        {
            Send(client, new PongResponse());
        }

        private void HandleJoin(Client client, string data)
        {
            if (client.Status != ClientStatus.Connected)
            {
                Send(client, new JoinResponse(JoinStatus.AlreadyJoined));
                return;
            }

            var nick = JsonConvert.DeserializeObject<JoinRequest>(data).Nick;
            if (IsNickTaken(nick))
            {
                Send(client, new JoinResponse(JoinStatus.NickExists));
                return;
            }

            client.Nick = nick;
            client.Status = ClientStatus.Joined;
            Send(client, new JoinResponse(JoinStatus.Success));
        }

        private void HandleSendInvite(Client client, string data)
        {
            var clientId = JsonConvert.DeserializeObject<InviteSendRequest>(data).PlayerId;
            if (clientId == client.Id)
            {
                Send(client, new InviteSendResponse(InviteSendStatus.SelfInvite));
                return;
            }

            Client invitedClient;
            lock (_lock)
            {
                invitedClient = _clients.FirstOrDefault(c => c.Id == clientId);
            }

            if (invitedClient == null)
            {
                Send(client, new InviteSendResponse(InviteSendStatus.PlayerNotExist));
                return;
            }

            Send(client, new InviteSendResponse(InviteSendStatus.Success));
            Send(invitedClient, new InviteSendRequest { PlayerId = client.Id });
        }

        private void HandleAnswerInvite(Client client, string data)
        {
            var response = JsonConvert.DeserializeObject<InviteAnswerResponse>(data);
            if (response.PlayerId == client.Id)
            {
                return;
            }

            Client invitedClient;
            lock (_lock)
            {
                invitedClient = _clients.FirstOrDefault(c => c.Id == response.PlayerId);
            }

            if (invitedClient == null)
            {
                return;
            }

            if (response.Status == InviteAnswerStatus.Accept)
            {
                // TODO: create game
            }
        }

        private void HandleGetPlayers(Client client, string data)
        {
            Client[] clients;
            lock (_lock)
            {
                clients = _clients.Where(c => c.Status == ClientStatus.Joined).ToArray();
            }

            Send(client, new OnlinePlayers
            {
                Players = clients
                    .Select(c => new PlayerItem
                    {
                        Id = c.Id,
                        Nick = c.Nick,
                        Status = c.Status
                    })
                    .ToArray()
            });
        }

        #endregion

        private void Send<T>(Client client, T message)
            where T : Message
        {
            _server.Send(client.Socket, MakeResponse(message));
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

            var json = JsonConvert.SerializeObject(value, Formatting.None);
            return $"{json}\n";
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