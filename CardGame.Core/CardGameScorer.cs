using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Utilities;

namespace CardGame.Core
{
    public class CardGameScorer : IScorer
    {
        private Func<ICard, int> getCardValueDelegate;
        private int winnerBonusPointTotal;

        public CardGameScorer(ScoringSettings settings)
        {
            Guard.AgainstNull(settings, nameof(settings));

            this.getCardValueDelegate = settings.GetCardValueDelegate;
            this.winnerBonusPointTotal = settings.WinnerBonusPointTotal;
        }

        public IEnumerable<int> Score(IEnumerable<ICard> cards)
        {
            Guard.AgainstNull(cards, nameof(cards));

            var scores = cards.Select(card => getCardValueDelegate(card)).ToArray();
            var highestScore = scores.Max();

            if (highestScore < 0)
            {
                return scores;
            }
            else
            {
                var cardScores = scores.Select(score =>
                    score == highestScore
                    ? winnerBonusPointTotal
                    : score < 0
                        ? score
                        : 0);

                return cardScores;
            }
        }
    }
}