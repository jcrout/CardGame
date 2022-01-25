using System.Linq;
using CardGame.Utilities;

namespace CardGame.Core
{
    public class VictoryChecker : IVictoryChecker
    {
        private int minimumVictoryScoreTotal;
        private int requiredScoreLead;
        private bool scoreLeadMustBeExactValue;

        public VictoryChecker(VictoryCheckerSettings settings)
        {
            Guard.AgainstNull(settings, nameof(settings));

            this.minimumVictoryScoreTotal = settings.MinimumVictoryScoreTotal;
            this.requiredScoreLead = settings.RequiredScoreLead;
            this.scoreLeadMustBeExactValue = settings.ScoreLeadMustBeExactValue;
        }

        public IPlayer GetVictoriousPlayer(IPlayerCollection players)
        {
            Guard.AgainstNull(players, nameof(players));
            Guard.AgainstNullOrEmptyEnumerable(players, nameof(players));

            var orderedPlayers = players.OrderByDescending(player => player.Score).ToList();
            if (orderedPlayers[0].Score >= this.minimumVictoryScoreTotal)
            {
                if (orderedPlayers.Count == 1)
                {
                    return orderedPlayers[0];
                }

                var scoreDifferential = (orderedPlayers[0].Score - orderedPlayers[1].Score);
                if (this.scoreLeadMustBeExactValue)
                {
                    if (scoreDifferential == this.requiredScoreLead)
                    {
                        return orderedPlayers[0];
                    }
                }
                else if (scoreDifferential > this.requiredScoreLead)
                {
                    return orderedPlayers[0];
                }
            }

            return null;
        }
    }
}