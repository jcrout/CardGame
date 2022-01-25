using System.Collections.Generic;

namespace CardGame.Core
{
    public interface IScorer
    {
        IEnumerable<int> Score(IEnumerable<ICard> cards);
    }
}