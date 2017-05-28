using Chess.Common.Messages;

namespace Chess.Server
{
    public class Invitation
    {
        public Client InvitingPlayer { get; set; }
        public Client InvitedPlayer { get; set; }
        public InviteAnswer? Answer { get; set; }
    }
}