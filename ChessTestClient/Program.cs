using System;
using System.IO;
using System.Net.Sockets;
using Chess.Common;
using Chess.Common.Messages;
using Newtonsoft.Json;

namespace Chess.Client.cs
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 11000);

            var networkStream = client.GetStream();
            var streamReader = new StreamReader(networkStream);
            var streamWriter = new StreamWriter(networkStream) { AutoFlush = true };

            streamWriter.Write(MessageHelpers.SerializeMessage(new JoinRequest("Mój TeStOwY NICK")));
            streamWriter.Write(MessageHelpers.SerializeMessage(new Message("get_players")));

            while (true)
            {
                if (networkStream.DataAvailable)
                {
                    var msg = streamReader.ReadLine();
                    switch (MessageHelpers.ExtractType(msg))
                    {
                        // Server's responses for sent requests
                        case "join":
                            var a = JsonConvert.DeserializeObject<JoinResponse>(msg);
                            Console.WriteLine($"join: {a.Status}");
                            break;
                        case "players":
                            var b = JsonConvert.DeserializeObject<GetPlayersResponse>(msg);
                            foreach (var player in b.Players)
                                Console.WriteLine($"player: {player.Nick} {player.Status} {player.Id}");
                            break;
                        case "answer_invite":
                            var c = JsonConvert.DeserializeObject<AnswerInviteResponse>(msg);
                            Console.WriteLine($"answer_invite: {c.PlayerId} {c.Status}");
                            break;
                        case "send_invite":
                            var d = JsonConvert.DeserializeObject<SendInviteResponse>(msg);
                            Console.WriteLine($"send_invite: {d.PlayerId} {d.Status}");
                            break;
                        case "move":
                            var e = JsonConvert.DeserializeObject<MoveResponse>(msg);
                            Console.WriteLine($"move: {e.Status}");
                            break;
                        case "end_turn":
                            var f = JsonConvert.DeserializeObject<EndTurnResponse>(msg);
                            Console.WriteLine($"end_turn: {f.GameId} {f.Status}");
                            break;

                        // Notifications - may occur any time
                        case "game":
                            var g = JsonConvert.DeserializeObject<GameNotification>(msg);
                            Console.WriteLine($"game created: {g.GameId} {g.WhitePlayerId} {g.BlackPlayerId}");
                            break;
                        case "invite":
                            var h = JsonConvert.DeserializeObject<InviteNotification>(msg);
                            Console.WriteLine($"got invitation: {h.PlayerId}");
                            break;
                        case "move_done":
                            var i = JsonConvert.DeserializeObject<MoveNotification>(msg);
                            Console.WriteLine($"got from player in game: {i.GameId} {i.Move}");
                            break;
                        case "player":
                            var j = JsonConvert.DeserializeObject<PlayerNotification>(msg);
                            Console.WriteLine($"player changed status, nick or just joined: {j.Player.Nick} {j.Player.Status} {j.Player.Id}");
                            break;
                        default:
                            Console.WriteLine($"unknown message, server sent: {msg?.Trim()}");
                            break;
                    }
                }
            }
        }
    }
}
