using System;

namespace CardGame.Core
{
    public class ScoringSettings
    {
        public Func<ICard, int> GetCardValueDelegate { get; set; }
        public int WinnerBonusPointTotal { get; set; }
    }
}