using System;
using System.Linq;
using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
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
}