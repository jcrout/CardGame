using System;
using CardGame.Core;
using CardGame.TextBased;
using CardGame.Utilities;

namespace CardGame.Console
{
    /// <summary>
    ///     This class is used to manage players and individual game sessions, including starting, stopping, and restarting games.
    /// </summary>
    public class CardGameSessionManager
    {
        private ICardGameFactory cardGameFactory;
        private IPlayerCollectionFactory playerCollectionFactory;
        private ITextInterface textInterface;
        private char continueRoundKey;
        private char exitKey;

        public CardGameSessionManager(KeyInputSettings settings, ITextInterface textInterface, IPlayerCollectionFactory playerCollectionFactory, ICardGameFactory cardGameFactory)
        {
            Guard.AgainstNullDataContainer(settings, nameof(settings));
            Guard.AgainstNull(textInterface, nameof(textInterface));
            Guard.AgainstNull(playerCollectionFactory, nameof(playerCollectionFactory));
            Guard.AgainstNull(cardGameFactory, nameof(cardGameFactory));

            this.continueRoundKey = settings.ContinueRoundKey;
            this.exitKey = settings.ExitKey;
            this.playerCollectionFactory = playerCollectionFactory;
            this.cardGameFactory = cardGameFactory;
            this.textInterface = textInterface;
            this.textInterface.KeyPressed += TextInterface_KeyPressed;
        }

        private void TextInterface_KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyPressed == this.exitKey)
            {
                this.Exit();
            }
        }

        private void Exit()
        {
            this.textInterface.KeyPressed -= TextInterface_KeyPressed;

            this.textInterface.WriteLine();
            this.textInterface.WriteLine();
            this.textInterface.WriteLine("Press any key to exit. ");
            this.textInterface.ReadKey();
            this.textInterface.Backspace();

            Environment.Exit(0);
        }

        public void BeginSession()
        {
            this.textInterface.WriteLine("~~ Card Game ~~");
            this.textInterface.WriteLine();

            var game = this.cardGameFactory.Create();
            this.ExecuteGame(game);
        }

        private void ExecuteGame(ICardGame game)
        {
            int roundCounter = 1;
            do
            {
                this.textInterface.Clear();
                this.textInterface.WriteLine($"Round {roundCounter}");
                this.textInterface.WriteLine();

                game.ExecuteRound();

                roundCounter++;
                if (!game.IsGameOver())
                {
                    this.textInterface.WriteLine();
                    this.textInterface.Write($"Press {this.continueRoundKey} to continue. ");
                    this.textInterface.ReadSingleCharFromList(new[] { this.continueRoundKey });
                    this.textInterface.Backspace();
                }
            }
            while (!game.IsGameOver());

            this.HandleGameOver(game);
        }

        private void HandleGameOver(ICardGame game)
        {
            this.textInterface.WriteLine();
            if (this.textInterface.GetYesOrNoResponse("Play again?"))
            {
                game.RestartGame();

                this.textInterface.WriteLine();
                if (!this.textInterface.GetYesOrNoResponse("Keep the same players again?"))
                {
                    this.textInterface.Clear();
                    var newPlayers = this.playerCollectionFactory.Create();
                    game.SetPlayers(newPlayers);
                }

                this.ExecuteGame(game);
            }
            else
            {
                this.Exit();
            }
        }
    }
}