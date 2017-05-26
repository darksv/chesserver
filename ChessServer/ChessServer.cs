using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ChessServer
{
    public class ChessServer
    {
        private readonly Action<object> _logger;
        private readonly Server _server = new Server();
        private readonly object _lock = new object();
        private readonly List<Client> _clients = new List<Client>();
        private readonly Dictionary<string, Action<Client, string>> _handlers;
        private readonly List<Game> _games = new List<Game>();
        private readonly List<Invitation> _invitations = new List<Invitation>();

        public ChessServer(Action<object> logger)
        {
            _logger = logger;
            _handlers = new Dictionary<string, Action<Client, string>>
            {
                {"join", HandleJoin},
                {"leave", HandleLeave},
                {"ping", HandlePing},
                {"send_invite", HandleSendInvite},
                {"answer_invite", HandleAnswerInvite},
                {"get_players", HandleGetPlayers},
                {"move", HandleMove},
                {"ready", HandleReady}
            };
        }

        public void Run(ushort port)
        {
            _server.Receive += ServerOnReceive;
            _server.Connect += ServerOnConnect;
            _server.Disconnect += ServerOnDisconnect;
            _server.Start(port);
        }

        private void Log(object value)
        {
            _logger.Invoke(value);
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

            Log($"We've lost connection with {args.ClientSocket.RemoteEndPoint} (Reason: {args.DisconnectionReason})");
        }

        private void ServerOnConnect(object sender, ConnectionEventArgs args)
        {
            var client = new Client(args.ClientSocket);
            lock (_lock)
            {
                _clients.Add(client);
            }

            Log($"We've established connection with {args.ClientSocket.RemoteEndPoint}");
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
                Log("Error: invalid message format");
                return;
            }

            var messageType = message?.Type ?? string.Empty;
            if (_handlers.TryGetValue(messageType, out var handler))
            {
                handler(client, args.Message);
            }
            else
            {
                Log("Error: unknown command");
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

            if (string.IsNullOrWhiteSpace(nick))
            {
                Send(client, new JoinResponse(JoinStatus.NickInvalid));
                return;
            }

            if (IsNickTaken(nick))
            {
                Send(client, new JoinResponse(JoinStatus.NickOccupied));
                return;
            }

            client.Nick = nick;
            client.Status = ClientStatus.Joined;
            Send(client, new JoinResponse(JoinStatus.Success));

            Log($"Client {client.Socket.RemoteEndPoint} joined to the server as {nick}");
        }

        private void HandleLeave(Client client, string args)
        {
            Log($"{client.Nick} has left the game");
        }

        private void HandleSendInvite(Client client, string data)
        {
            var clientId = JsonConvert.DeserializeObject<SendInviteRequest>(data).PlayerId;
            if (clientId == client.Id)
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.SelfInvite));
                return;
            }

            Client invitedClient;
            lock (_lock)
            {
                invitedClient = _clients.FirstOrDefault(c => c.Id == clientId);
            }

            if (invitedClient == null)
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.PlayerNotExist));
                return;
            }

            if (_invitations.Any(i => i.InvitedPlayer == invitedClient && i.InvitingPlayer == client && !i.Answer.HasValue))
            {
                Send(client, new SendInviteResponse(clientId, SendInviteStatus.AlreadyInvited));
                return;
            }

            _invitations.Add(new Invitation
            {
                InvitingPlayer = client,
                InvitedPlayer = invitedClient,
            });

            Send(client, new SendInviteResponse(clientId, SendInviteStatus.Success));
            Send(invitedClient, new InviteNotification(client.Id));

            Log($"{client.Nick} has invited {invitedClient.Nick} to the game");
        }

        private void HandleAnswerInvite(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<AnswerInviteRequest>(data);
            if (request.PlayerId == client.Id)
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.InvalidPlayer));
                return;
            }

            Client invitingClient;
            lock (_lock)
            {
                invitingClient = _clients.FirstOrDefault(c => c.Id == request.PlayerId);
            }

            if (invitingClient == null)
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.InvalidPlayer));
                return;
            }

            if (!_invitations.Any(i => i.InvitedPlayer == client && i.InvitingPlayer == invitingClient &&
                                       !i.Answer.HasValue))
            {
                Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.NotInvited));
                return;
            }

            if (request.Answer == InviteAnswer.Accept)
            {
                var game = new Game(client, invitingClient);
                client.Game = game;
                client.Status = ClientStatus.OnGame;

                invitingClient.Game = game;
                invitingClient.Status = ClientStatus.OnGame;

                _games.Add(game);

                Log($"{client.Nick} has accepted {invitingClient.Nick}'s invitation");
                Log($"Created game: {client.Nick} with {invitingClient.Nick}");
            }
            else
            {
                Log($"{client.Nick} has rejected {invitingClient.Nick}'s invitation");
            }

            Send(client, new AnswerInviteResponse(request.PlayerId, AnswerInviteStatus.Success));
        }

        private void HandleGetPlayers(Client client, string data)
        {
            PlayerItem[] players;
            lock (_lock)
            {
                players = _clients
                    .Where(c => c.Status == ClientStatus.Joined)
                    .Select(c => new PlayerItem
                    {
                        Id = c.Id,
                        Nick = c.Nick,
                        Status = c.Status
                    })
                    .ToArray();
            }

            Send(client, new GetPlayersResponse(players));
        }

        private void HandleMove(Client client, string data)
        {
            var request = JsonConvert.DeserializeObject<MoveRequest>(data);

            if (client.Status != ClientStatus.OnGame)
            {
                return;
            }

            var game = _games
                .FirstOrDefault(g => g.InvolvesPlayer(client));

            if (game == null)
            {
                Send(client, new MoveResponse(MoveStatus.NotOnGame));
                return;
            }

            // TODO: check move

            Send(client, new MoveResponse(MoveStatus.Success));
            Send(game.GetOpponentFor(client), new MoveNotification(client.Id, request.Move));
        }

        private void HandleReady(Client client, string data)
        {
            if (client.Status != ClientStatus.OnGame)
            {
                return;
            }

            Log($"Player {client.Nick} finished his move!");
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