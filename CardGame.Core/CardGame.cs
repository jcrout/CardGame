using CardGame.Utilities;

namespace CardGame.Core
{
    public class CardGame : ICardGame
    {
        private ICommandHandler<DrawCardsCommand> drawCardsCommandHandler;
        private ICommandHandler<UpdateScoresCommand> updateScoresCommand;
        private ICommandHandler<CheckForGameEndCommand> handleGameEndCommand;
        private IPlayerCollection players;
        private IDeck deck;
        private IScorer scorer;
        private IVictoryChecker victoryChecker;
        private bool isGameOver = false;

        public CardGame(CardGameCommands commands, IPlayerCollection players, IDeck deck, IScorer scorer, IVictoryChecker victoryChecker)
        {
            Guard.AgainstNullDataContainer(commands, nameof(commands));
            Guard.AgainstNull(players, nameof(players));
            Guard.AgainstNull(deck, nameof(deck));
            Guard.AgainstNull(scorer, nameof(scorer));
            Guard.AgainstNull(victoryChecker, nameof(victoryChecker));

            this.drawCardsCommandHandler = commands.DrawCardsCommandHandler;
            this.updateScoresCommand = commands.UpdateScoresCommandHandler;
            this.handleGameEndCommand = commands.CheckForGameEndCommandHandler;
            this.players = players;
            this.deck = deck;
            this.scorer = scorer;
            this.victoryChecker = victoryChecker;
        }

        public void ExecuteRound()
        {
            var drawCardsCommand = new DrawCardsCommand()
            {
                Deck = this.deck,
                Players = this.players
            };

            this.drawCardsCommandHandler.Handle(drawCardsCommand);
            var cardsDrawn = drawCardsCommand.CardsDrawn;

            var updateScoresCommand = new UpdateScoresCommand()
            {
                Players = this.players,
                Scorer = this.scorer,
                CardsDrawn = cardsDrawn
            };

            this.updateScoresCommand.Handle(updateScoresCommand);

            var handleGameEndCommand = new CheckForGameEndCommand()
            {
                Players = this.players,
                VictoryChecker = this.victoryChecker
            };

            this.handleGameEndCommand.Handle(handleGameEndCommand);
            this.isGameOver = handleGameEndCommand.IsGameOver;
        }

        public bool IsGameOver()
        {
            return this.isGameOver;
        }

        public void RestartGame()
        {
            this.isGameOver = false;

            foreach (var player in this.players)
            {
                player.Score = 0;
            }
        }

        public void SetPlayers(IPlayerCollection players)
        {
            Guard.AgainstNull(players, nameof(players));

            this.players = players;
        }
    }
}