using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;
using CardGame.Utilities;

namespace CardGame.Core
{
    internal static class Extensions
    {
        public static Stack<T> ToStack<T>(this IEnumerable<T> @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            return new Stack<T>(@this);
        }
    }

    public interface IDeck
    {
        void Shuffle();

        ICard Draw();
    }

    public interface IDeckFactory
    {
        IDeck Create();
    }

    public interface ICard
    {
        IFaceValue FaceValue { get; set; }
        ISuit Suit { get; set; }
    }

    public interface IFaceValue
    {
        string Value { get; set; }
    }

    public interface ISuit
    {
        string Name { get; set; }
    }

    public interface IPlayer
    {
        string Name { get; set; }
        int Score { get; set; }
    }

    public interface IPlayerFactory
    {
        IPlayer Create(string playerName);
    }

    public interface IPlayerCollectionFactory
    {
        IPlayerCollection Create();
    }

    public interface ICardGame
    {
        void ExecuteRound();

        void RestartGame();

        void SetPlayers(IPlayerCollection players);

        bool IsGameOver();
    }

    public interface ICardGameFactory
    {
        ICardGame Create();
    }

    public interface IScorer
    {
        IEnumerable<int> Score(IEnumerable<ICard> cards);
    }

    public interface IScorerFactory
    {
        IScorer Create();
    }

    public interface IVictoryCheckerFactory
    {
        IVictoryChecker Create();
    }

    public interface IVictoryChecker
    {
        IPlayer GetVictoriousPlayer(IPlayerCollection players);
    }

    public interface IPlayerCollection : ICollection<IPlayer>
    {

    }

    public interface ICardCollectionFactory
    {
        ICardCollection Create();
    }

    public interface ICardCollection : ICollection<ICard>
    {
    }

    public interface IRandomGenerator
    {
        int GetRandomInteger(int minimumInclusive, int maximumExclusive);
    }

    public interface ICommand
    {
    }

    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }

    public class RandomGenerator : IRandomGenerator
    {
        private Random random = new Random();

        public int GetRandomInteger(int minimumInclusive, int maximumExclusive)
        {
            return random.Next(minimumInclusive, maximumExclusive);
        }
    }

    public class Deck : IDeck
    {
        private ICardCollection cards;

        private Stack<ICard> currentCards;
        private IRandomGenerator randomGenerator;

        public Deck(ICardCollection cards, IRandomGenerator randomGenerator)
        {
            Guard.AgainstNull(cards, nameof(cards));
            Guard.AgainstNull(randomGenerator, nameof(randomGenerator));

            this.cards = cards;
            this.randomGenerator = randomGenerator;
            this.Shuffle();
        }

        public ICard Draw()
        {
            var card = currentCards.Pop();
            return card;
        }

        public void Shuffle()
        {
            this.currentCards = this.cards.OrderBy(card => this.randomGenerator.GetRandomInteger(0, Int32.MaxValue)).ToStack();
        }
    }

    public class Card : ICard
    {
        public IFaceValue FaceValue { get; set; }

        public ISuit Suit { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}{1}",
                FaceValue.Value,
                this.Suit != null ? " of " + this.Suit.Name : "");
        }
    }

    public class Suit : ISuit
    {
        public string Name { get; set; }
    }

    public class FaceValue : IFaceValue
    {
        public string Value { get; set; }
    }

    public class Player : IPlayer
    {
        public string Name { get; set; }

        public int Score { get; set; }
    }

    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer Create(string playerName)
        {
            Guard.AgainstNullOrWhiteSpaceString(playerName, nameof(playerName));

            return new Player() { Name = playerName };
        }
    }

    public class PlayerCollection : List<IPlayer>, IPlayerCollection
    {
    }

    public class PlayerCollectionFactory : IPlayerCollectionFactory
    {
        public IPlayerCollection Create()
        {
            return new PlayerCollection();
        }
    }

    public class CardCollection : List<ICard>, ICardCollection
    {
    }

    public class ScoringSettings
    {
        public Func<ICard, int> GetCardValueDelegate { get; set; }
        public int WinnerBonusPointTotal { get; set; }
    }

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

    public class VictoryCheckerSettings
    {
        public int MinimumVictoryScoreTotal { get; set; }
        public int RequiredScoreLead { get; set; }
        public bool ScoreLeadMustBeExactValue { get; set; }
    }

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

    public class DeckSettings
    {
        public ICardCollection Cards { get; set; }
    }

    public class DeckFactory : IDeckFactory
    {
        private IRandomGenerator randomGenerator;
        private ICardCollection cards;

        public DeckFactory(DeckSettings settings, IRandomGenerator randomGenerator)
        {
            Guard.AgainstNull(settings, nameof(settings));
            Guard.AgainstNull(randomGenerator, nameof(randomGenerator));

            this.cards = settings.Cards;
            this.randomGenerator = randomGenerator;
        }

        public IDeck Create()
        {
            return new Deck(cards, randomGenerator);
        }
    }

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

    public class DrawCardsCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public IDeck Deck { get; set; }

        public ICardCollection CardsDrawn { get; set; }
    }

    public class UpdateScoresCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public ICardCollection CardsDrawn { get; set; }

        public IScorer Scorer { get; set; }
    }

    public class CheckForGameEndCommand : ICommand
    {
        public IPlayerCollection Players { get; set; }

        public IVictoryChecker VictoryChecker { get; set; }

        public bool IsGameOver { get; set; }
    }

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
