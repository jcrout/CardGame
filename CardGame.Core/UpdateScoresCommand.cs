namespace CardGame.Core
{
    public class UpdateScoresCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public ICardCollection CardsDrawn { get; set; }

        public IScorer Scorer { get; set; }
    }
}