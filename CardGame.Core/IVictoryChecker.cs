namespace CardGame.Core
{
    public interface IVictoryChecker
    {
        IPlayer GetVictoriousPlayer(IPlayerCollection players);
    }
}