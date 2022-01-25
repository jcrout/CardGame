using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.Console
{
    /// <summary>
    ///     Factory class used to create new instances of the <see cref="CardGame.Core.CardGame"/> class.
    /// </summary>
    public class CardGameFactory : ICardGameFactory
    {
        private IPlayerCollectionFactory playerCollectionFactory;
        private IDeckFactory deckFactory;
        private IVictoryCheckerFactory victoryCheckerFactory;
        private IScorerFactory scorerFactory;
        private CardGameCommands commands;

        public CardGameFactory(CardGameCommands commands, IPlayerCollectionFactory playerCollectionFactory, IDeckFactory deckFactory, IScorerFactory scorerFactory, IVictoryCheckerFactory victoryCheckerFactory)
        {
            Guard.AgainstNullDataContainer(commands, nameof(commands));
            Guard.AgainstNull(playerCollectionFactory, nameof(playerCollectionFactory));
            Guard.AgainstNull(deckFactory, nameof(deckFactory));
            Guard.AgainstNull(scorerFactory, nameof(scorerFactory));
            Guard.AgainstNull(victoryCheckerFactory, nameof(victoryCheckerFactory));

            this.commands = commands;
            this.playerCollectionFactory = playerCollectionFactory;
            this.deckFactory = deckFactory;
            this.victoryCheckerFactory = victoryCheckerFactory;
            this.scorerFactory = scorerFactory;
        }

        public ICardGame Create()
        {
            return new CardGame.Core.CardGame(this.commands, this.playerCollectionFactory.Create(), this.deckFactory.Create(), this.scorerFactory.Create(), this.victoryCheckerFactory.Create());
        }
    }
}