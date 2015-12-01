using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public static class Extensions
    {
        public static void WriteLine(this ITextInterface @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            @this.WriteLine(String.Empty);
        }

        public static int ReadIntegerInRange(this ITextInterface @this, int minimum, int maximum)
        {
            Guard.AgainstNull(@this, nameof(@this));

            if (maximum < minimum)
            {
                throw new ArgumentException($"{nameof(minimum)} must be less than or equal to {nameof(maximum)}.");
            }

            int value = 0;
            int startIndex = @this.CursorLeft;
            var input = "";

            while (true)
            {
                var inputChar = @this.ReadKey();
                if (char.IsNumber(inputChar))
                {
                    input += inputChar;
                    var yoyo = (int)inputChar;
                    value = Int32.Parse(input);
                    if (value >= minimum)
                    {
                        if (value <= maximum)
                        {
                            return value;
                        }
                        else
                        {
                            input = input.Substring(0, input.Length - 1);
                            @this.Backspace();
                        }
                    }
                }
                else if (inputChar == 8)
                {
                    if (@this.CursorLeft == startIndex - 1)
                    {
                        @this.Write(" ");
                    }
                    else
                    {
                        input = (input.Length > 0) ? input.Substring(0, input.Length - 1) : input;
                        @this.Write(" \b");
                    }
                }
                else
                {
                    @this.Backspace();
                }
            }
        }

        public static char ReadSingleCharFromList(this ITextInterface @this, params char[] validChars)
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNullOrEmptyEnumerable(validChars, nameof(validChars));

            var upperValidChars = validChars.Select(chr => Char.IsLower(chr) ? Char.ToUpper(chr) : chr);
            while (true)
            {
                var inputChar = @this.ReadKey();
                if (char.IsLower(inputChar))
                {
                    inputChar = char.ToUpper(inputChar);
                }

                if (upperValidChars.Contains(inputChar))
                {
                    return inputChar;
                }
                else
                {
                    @this.Backspace();
                }
            }
        }

        public static bool GetYesOrNoResponse(this ITextInterface @this, string questionText, char yesChar = 'Y', char noChar = 'N')
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNull(questionText, nameof(questionText));

            @this.Write($"{questionText} ({yesChar}|{noChar}): ");
            return ReadSingleCharFromList(@this, new char[] { yesChar, noChar }) == yesChar;
        }

        public static void ClearCurrentLine(this ITextInterface @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            @this.SetLine(@this.CursorTop, new string(' ', @this.WindowWidth));
            @this.CursorLeft = @this.BufferLeft;
        }

        public static string ReadLine(this ITextInterface @this, int maximumLength, int minimumLength = 0)
        {
            Guard.AgainstNull(@this, nameof(@this));

            var builder = new StringBuilder();

            while (true)
            {
                var inputKey = @this.ReadKey();

                if (inputKey == 13)
                {
                    if (builder.Length >= minimumLength)
                    {
                        break;
                    }
                    else
                    {
                        @this.Backspace();
                    }
                }
                else if (inputKey == 8)
                {
                    if (builder.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                    }
                    else
                    {
                        @this.Write(" ");
                    }
                }
                else if (builder.Length < maximumLength)
                {
                    builder.Append(inputKey);
                }
                else
                {
                    @this.Backspace();
                }
            }

            return builder.ToString();
        }

        public static void SetLine(this ITextInterface @this, int lineIndex, string text)
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNull(text, nameof(text));

            if (text.Length > @this.WindowWidth)
            {
                throw new ArgumentException($"{nameof(text)} must be less than {nameof(ITextInterface)}.{nameof(@this.WindowWidth)}");
            }

            int currentLine = @this.CursorTop;
            int currentColumn = @this.CursorLeft;

            @this.CursorTop = lineIndex;
            @this.CursorLeft = 0;

            @this.Write(text);

            if (text.Length < @this.WindowWidth)
            {
                for (int i = text.Length; i < @this.WindowWidth; i++)
                {
                    @this.Write(" ");
                }
            }

            @this.CursorTop = currentLine;
            @this.CursorLeft = currentColumn;
        }
    }

    public interface ITextInterface
    {
        event EventHandler<KeyPressEventArgs> KeyPressed;

        string Title { get; set; }

        int BufferTop { get; set; }

        int BufferLeft { get; set; }

        int CursorLeft { get; set; }

        int CursorTop { get; set; }

        int WindowWidth { get; set; }

        int WindowHeight { get; set; }

        void Write(string text);

        void WriteLine(string text);

        void Clear();

        void Backspace();

        char ReadKey();

        string ReadLine();
    }

    public class KeyPressEventArgs : System.EventArgs
    {
        public char KeyPressed { get; }

        public bool Handled { get; set; } = false;

        public KeyPressEventArgs(char keyPressed)
        {
            this.KeyPressed = keyPressed;
        }
    }

    public class PlayerSettings
    {
        public int MinimumNameLength { get; set; }
        public int MaximumNameLength { get; set; }
        public int MinimumPlayerCount { get; set; }
        public int MaximumPlayerCount { get; set; }
    }

    public class PlayerCollectionFactory : IPlayerCollectionFactory
    {
        private IPlayerFactory playerFactory;
        private ITextInterface textInterface;
        private PlayerSettings playerSettings;

        public PlayerCollectionFactory(ITextInterface textInterface, IPlayerFactory playerFactory, PlayerSettings playerSettings)
        {
            Guard.AgainstNull(textInterface, nameof(textInterface));
            Guard.AgainstNull(playerFactory, nameof(playerFactory));
            Guard.AgainstNullDataContainer(playerSettings, nameof(playerSettings));

            this.playerFactory = playerFactory;
            this.textInterface = textInterface;
            this.playerSettings = playerSettings;
        }

        public IPlayerCollection Create()
        {
            this.textInterface.Write($"Enter the number of players, between {this.playerSettings.MinimumPlayerCount} and {this.playerSettings.MaximumPlayerCount}: ");
            var playerCount = this.textInterface.ReadIntegerInRange(
                this.playerSettings.MinimumPlayerCount,
                this.playerSettings.MaximumPlayerCount);

            this.textInterface.WriteLine();
            var nameEachPlayer = this.textInterface.GetYesOrNoResponse("Enter player names?");

            var playerCollection = new PlayerCollection() { Capacity = playerCount };
            if (nameEachPlayer)
            {
                this.textInterface.WriteLine();
                this.textInterface.WriteLine();
                this.textInterface.WriteLine($"Enter a unique name for each player, between {this.playerSettings.MinimumNameLength} and {this.playerSettings.MaximumNameLength} characters.");
                for (int i = 0; i < playerCount; i++)
                {
                    this.textInterface.Write($"Player {i + 1}: ");
                    while (true)
                    {
                        var line = this.textInterface.ReadLine(this.playerSettings.MaximumNameLength, this.playerSettings.MinimumNameLength);
                        if (String.IsNullOrWhiteSpace(line) || playerCollection.FirstOrDefault(player => String.Equals(player.Name, line)) != null)
                        {
                            this.textInterface.WriteLine("Error: Player name must be unique and contain at least one character.");
                            this.textInterface.Write($"Enter a unique name for Player {i + 1}: ");
                        }
                        else
                        {
                            playerCollection.Add(this.playerFactory.Create(line));
                            break;
                        }
                    }
                }
            }
            else
            {
                var players = Enumerable.Range(1, playerCount)
                              .Select(index => playerFactory.Create("Player " + index));
                playerCollection.AddRange(players);
            }

            return playerCollection;
        }
    }

    public class KeyInputSettings
    {
        public char DrawCardKey { get; set; }

        public char ContinueRoundKey { get; set; }

        public char ExitKey { get; set; }
    }

    public abstract class TextCardGameCommmandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected ITextInterface TextInterface { get; }

        public TextCardGameCommmandHandler(ITextInterface textInterface)
        {
            Guard.AgainstNull(textInterface, nameof(textInterface));

            this.TextInterface = textInterface;
        }

        public abstract void Handle(TCommand command);
    }

    public class DrawCardsCommandHandler : TextCardGameCommmandHandler<DrawCardsCommand>
    {
        private ICardCollectionFactory cardCollectionFactory;
        private char drawCardKey;

        public DrawCardsCommandHandler(KeyInputSettings settings, ITextInterface textInterface, ICardCollectionFactory cardCollectionFactory) : base(textInterface)
        {
            Guard.AgainstNullDataContainer(settings, nameof(settings));
            Guard.AgainstNull(textInterface, nameof(textInterface));
            Guard.AgainstNull(cardCollectionFactory, nameof(cardCollectionFactory));

            this.drawCardKey = settings.DrawCardKey;
            this.cardCollectionFactory = cardCollectionFactory;
        }

        public override void Handle(DrawCardsCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            command.Deck.Shuffle();

            var cardsDrawnThisRound = this.cardCollectionFactory.Create();
            foreach (var player in command.Players)
            {
                this.TextInterface.Write($"{player.Name}, press {this.drawCardKey} to draw a card. ");
                this.TextInterface.ReadSingleCharFromList(new[] { this.drawCardKey });
                this.TextInterface.ClearCurrentLine();

                var drawnCard = command.Deck.Draw();
                var cardHasASuit = drawnCard.Suit != null;
                cardsDrawnThisRound.Add(drawnCard);

                this.TextInterface.Write($"{player.Name} drew ");

                if (cardHasASuit)
                {
                    this.TextInterface.Write("the ");
                    this.TextInterface.Write(drawnCard.FaceValue.Value);
                    this.TextInterface.Write($" of ");
                    this.TextInterface.Write(drawnCard.Suit.Name);
                    this.TextInterface.WriteLine(".");
                }
                else
                {
                    this.TextInterface.Write(StringUtility.AorAn(drawnCard.FaceValue.Value));
                    this.TextInterface.WriteLine(" card.");
                }
            }

            command.CardsDrawn = cardsDrawnThisRound;
        }
    }

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
