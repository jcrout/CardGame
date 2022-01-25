using CardGame.Utilities;

namespace CardGame.Core
{
    public class DeckFactory : IDeckFactory
    {
        private IRandomGenerator randomGenerator;
        private ICardCollection cards;

        public DeckFactory(DeckSettings settings, IRandomGenerator randomGenerator)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNull(randomGenerator, nameof(randomGenerator));

            this.cards = settings.Cards;
            this.randomGenerator = randomGenerator;
        }

        public IDeck Create()
        {
            return new Deck(cards, randomGenerator);
        }
    }
}