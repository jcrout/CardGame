using CardGame.Utilities;

namespace CardGame.Core
{
    public class CardGameCommands
    {
        public ICommandHandler<DrawCardsCommand> DrawCardsCommandHandler { get; }
        public ICommandHandler<UpdateScoresCommand> UpdateScoresCommandHandler { get; }
        public ICommandHandler<CheckForGameEndCommand> CheckForGameEndCommandHandler { get; }

        public CardGameCommands(ICommandHandler<DrawCardsCommand> drawCardsCommandHandler, ICommandHandler<UpdateScoresCommand> updateScoresCommand, ICommandHandler<CheckForGameEndCommand> handleGameEndCommand)
        {
            Guard.AgainstNull(drawCardsCommandHandler, nameof(drawCardsCommandHandler));
            Guard.AgainstNull(updateScoresCommand, nameof(updateScoresCommand));
            Guard.AgainstNull(handleGameEndCommand, nameof(handleGameEndCommand));

            this.DrawCardsCommandHandler = drawCardsCommandHandler;
            this.UpdateScoresCommandHandler = updateScoresCommand;
            this.CheckForGameEndCommandHandler = handleGameEndCommand;
        }
    }
}