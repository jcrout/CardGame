using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.Console
{
    internal class SpecialCardData
    {
        public ICard Card { get; set; }

        public int Count { get; set; }

        public int ScoreChange { get; set; }

        public SpecialCardData(ICard card, int scoreChange, int count)
        {
            Guard.AgainstNull(card, nameof(card));

            this.Card = card;
            this.ScoreChange = scoreChange;
            this.Count = count;
        }
    }
}