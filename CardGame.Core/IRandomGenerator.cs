namespace CardGame.Core
{
    public interface IRandomGenerator
    {
        int GetRandomInteger(int minimumInclusive, int maximumExclusive);
    }
}