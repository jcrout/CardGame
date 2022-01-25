using System;

namespace CardGame.Utilities
{
    /// <summary>
    ///     Utility class containing string helper methods.
    /// </summary>
    public static class StringUtility
    {
        /// <summary>
        ///     Prefixes the text with either 'a' or 'an' (or 'A' or 'An' if capitalization is specificed) and a space,
        ///     depending on the first letter of the text.
        /// </summary>
        /// <param name="text">The text to prefix.</param>
        /// <param name="capitalizeA">true to capitalize 'a' or 'an'; otherwise, 'a' or 'an' are used in lowercase.</param>
        /// <returns>The original text prefixed by either 'a' or 'an' (or 'A' or 'An' if capitalization is specificed) and a space.</returns>
        public static string AorAn(string text, bool capitalizeA = false)
        {
            var prefix = capitalizeA ? 'A' : 'a';

            if (String.IsNullOrWhiteSpace(text))
            {
                return prefix + " ";
            }

            var firstChar = Char.ToUpper(text[0]);
            if (firstChar == 'A' || firstChar == 'E' || firstChar == 'I' || firstChar == 'O' || firstChar == 'U')
            {
                return prefix + "n " + text;
            }
            else
            {
                return prefix + " " + text;
            }
        }
    }
}