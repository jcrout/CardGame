namespace CardGame.Core
{
    public class CheckForGameEndCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public IVictoryChecker VictoryChecker { get; set; }

        public bool IsGameOver { get; set; }
    }
}