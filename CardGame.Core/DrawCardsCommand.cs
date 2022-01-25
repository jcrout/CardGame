namespace CardGame.Core
{
    public class DrawCardsCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public IDeck Deck { get; set; }

        public ICardCollection CardsDrawn { get; set; }
    }
}