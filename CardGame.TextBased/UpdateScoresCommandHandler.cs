using System;
using System.Linq;
using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public class UpdateScoresCommandHandler : TextCardGameCommmandHandler<UpdateScoresCommand>
    {
        public UpdateScoresCommandHandler(ITextInterface textInterface) : base(textInterface)
        {
        }

        public override void Handle(UpdateScoresCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            var roundScoreChanges = command.Scorer.Score(command.CardsDrawn);
            var scoreEnumerator = roundScoreChanges.GetEnumerator();
            var highestRoundScore = roundScoreChanges.Max();
            IPlayer roundVictor = null;

            foreach (var player in command.Players)
            {
                scoreEnumerator.MoveNext();
                player.Score = Math.Max(0, player.Score + scoreEnumerator.Current);
                if (scoreEnumerator.Current > 0)
                {
                    roundVictor = player;
                }
            }

            this.TextInterface.WriteLine();
            if (roundVictor != null)
            {
                this.TextInterface.WriteLine($"{roundVictor.Name} drew the best card and was awarded {highestRoundScore} points.");
            }
            else
            {
                this.TextInterface.WriteLine($"No players received a positive score this round.");
            }

            this.TextInterface.WriteLine();
            this.TextInterface.WriteLine("Current scores:");

            var rankedPlayers = command.Players.OrderByDescending(player => player.Score).ToArray();
            for (int i = 0; i < rankedPlayers.Length; i++)
            {
                var player = rankedPlayers[i];
                this.TextInterface.WriteLine($"{i + 1}. {player.Name}: {player.Score}");
            }
        }
    }
}