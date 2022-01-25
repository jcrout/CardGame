using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Utilities;

namespace CardGame.Core
{
    public class Deck : IDeck
    {
        private ICardCollection cards;

        private Stack<ICard> currentCards;
        private IRandomGenerator randomGenerator;

        public Deck(ICardCollection cards, IRandomGenerator randomGenerator)
        {
            Guard.AgainstNull(cards, nameof(cards));
            Guard.AgainstNull(randomGenerator, nameof(randomGenerator));

            this.cards = cards;
            this.randomGenerator = randomGenerator;
            this.Shuffle();
        }

        public ICard Draw()
        {
            var card = currentCards.Pop();
            return card;
        }

        public void Shuffle()
        {
            this.currentCards = this.cards.OrderBy(card => this.randomGenerator.GetRandomInteger(0, Int32.MaxValue)).ToStack();
        }
    }
}