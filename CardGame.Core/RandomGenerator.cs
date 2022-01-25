using System;

namespace CardGame.Core
{
    public class RandomGenerator : IRandomGenerator
    {
        private Random random = new Random();

        public int GetRandomInteger(int minimumInclusive, int maximumExclusive)
        {
            return random.Next(minimumInclusive, maximumExclusive);
        }
    }
}