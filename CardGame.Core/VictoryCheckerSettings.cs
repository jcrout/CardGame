namespace CardGame.Core
{
    public class VictoryCheckerSettings
    {
        public int MinimumVictoryScoreTotal { get; set; }
        public int RequiredScoreLead { get; set; }
        public bool ScoreLeadMustBeExactValue { get; set; }
    }
}