using System.Collections.Generic;
using CardGame.Utilities;

namespace CardGame.Core
{
    internal static class Extensions
    {
        public static Stack<T> ToStack<T>(this IEnumerable<T> @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            return new Stack<T>(@this);
        }
    }
}
