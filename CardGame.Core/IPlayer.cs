namespace CardGame.Core
{
    public interface IPlayer
    {
        string Name { get; set; }
        int Score { get; set; }
    }
}