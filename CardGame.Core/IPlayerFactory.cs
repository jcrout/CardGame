namespace CardGame.Core
{
    public interface IPlayerFactory
    {
        IPlayer Create(string playerName);
    }
}