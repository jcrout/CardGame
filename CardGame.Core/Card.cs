namespace CardGame.Core
{
    public class Card : ICard
    {
        public IFaceValue FaceValue { get; set; }

        public ISuit Suit { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}{1}",
                FaceValue.Value,
                this.Suit != null ? " of " + this.Suit.Name : "");
        }
    }
}