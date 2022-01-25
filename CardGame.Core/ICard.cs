namespace CardGame.Core
{
    public interface ICard
    {
        IFaceValue FaceValue { get; set; }
        ISuit Suit { get; set; }
    }
}