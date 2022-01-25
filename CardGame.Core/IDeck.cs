namespace CardGame.Core
{
    public interface IDeck
    {
        void Shuffle();

        ICard Draw();
    }
}
