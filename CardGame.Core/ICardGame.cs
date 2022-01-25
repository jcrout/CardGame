namespace CardGame.Core
{
    public interface ICardGame
    {
        void ExecuteRound();

        void RestartGame();

        void SetPlayers(IPlayerCollection players);

        bool IsGameOver();
    }
}