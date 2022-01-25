using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public class CheckForGameEndCommandHandler : TextCardGameCommmandHandler<CheckForGameEndCommand>
    {
        public CheckForGameEndCommandHandler(ITextInterface textInterface) : base(textInterface)
        {
        }

        public override void Handle(CheckForGameEndCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            var victoriousPlayer = command.VictoryChecker.GetVictoriousPlayer(command.Players);
            if (victoriousPlayer != null)
            {
                this.TextInterface.WriteLine();
                this.TextInterface.WriteLine($"{victoriousPlayer.Name} has won with a score of {victoriousPlayer.Score}!");
                command.IsGameOver = true;
            }
        }
    }
}