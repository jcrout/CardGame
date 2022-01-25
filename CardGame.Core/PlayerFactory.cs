using CardGame.Utilities;

namespace CardGame.Core
{
    public class PlayerFactory : IPlayerFactory
    {
        public IPlayer Create(string playerName)
        {
            Guard.AgainstNullOrWhiteSpaceString(playerName, nameof(playerName));

            return new Player() { Name = playerName };
        }
    }
}