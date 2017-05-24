namespace ChessServer
{
    public class Game
    {
        private readonly Client _firstPlayer;
        private readonly Client _secondPlayer;

        public Game(Client firstPlayer, Client secondPlayer)
        {
            _firstPlayer = firstPlayer;
            _secondPlayer = secondPlayer;
        }
    }
}