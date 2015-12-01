namespace CardGame.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     This class contains static methods to simplify checking for null/empty conditions.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="parameterValue"/> is null.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to check.</typeparam>
        /// <param name="parameterValue">The parameter to check for null.</param>
        /// <param name="name">The name of the parameter. If null, <see cref="String.Empty"/> is used instead.</param>
        public static void AgainstNull<T>(T parameterValue, string name) where T : class
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(name ?? String.Empty);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="parameterValue"/> is null. 
        ///     If <paramref name="parameterValue"/> is empty, this method instead throws an 
        ///     <see cref="ArgumentException"/> with the value of <paramref name="isEmptyMessage"/>.
        /// </summary>
        /// <param name="parameterValue">The string parameter to check if null or empty.</param>
        /// <param name="name">The name of the string parameter. If null, <see cref="String.Empty"/> is used instead.</param>
        /// <param name="isEmptyMessage">The message to display in the exception if the string parameter is empty. 
        ///     If null, <see cref="String.Empty"/> is used instead.</param>
        public static void AgainstNullOrEmptyString(string parameterValue, string name, string isEmptyMessage = "String cannot be empty.")
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(name ?? String.Empty);
            }

            if (String.Equals(parameterValue, String.Empty))
            {
                throw new ArgumentException(isEmptyMessage ?? String.Empty, name ?? String.Empty);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="parameterValue"/> is null. 
        ///     If <paramref name="parameterValue"/> is composed only of whitespce, this method instead throws an 
        ///     <see cref="ArgumentException"/> with the value of <paramref name="isWhiteSpaceMessage"/>.
        /// </summary>
        /// <param name="parameterValue">The string parameter to check if null or empty.</param>
        /// <param name="name">The name of the string parameter. If null, <see cref="String.Empty"/> is used instead.</param>
        /// <param name="isWhiteSpaceMessage">The message to display in the exception if the string parameter is composed only of whitespace. 
        ///     If null, <see cref="String.Empty"/> is used instead.</param>
        public static void AgainstNullOrWhiteSpaceString(string parameterValue, string name, string isWhiteSpaceMessage = "String cannot contain only whitespace characters.")
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(name ?? String.Empty);
            }

            if (String.IsNullOrWhiteSpace(parameterValue))
            {
                throw new ArgumentException(isWhiteSpaceMessage ?? String.Empty, name ?? String.Empty);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="parameterValue"/> is null.
        /// </summary>
        /// <typeparam name="T">The type of the element contained in the <paramref name="parameterValue"/>.</typeparam>
        /// <param name="parameterValue"></param>
        /// <param name="name">The name of the enumerable parameter. If null, <see cref="String.Empty"/> is used instead.</param>
        /// <param name="isEmptyMessage">The message to display in the exception if the enumerable contains no elements. 
        ///     If null, <see cref="String.Empty"/> is used instead.</param>
        public static void AgainstNullOrEmptyEnumerable<T>(IEnumerable<T> parameterValue, string name, string isEmptyMessage = "Enumerable cannot be empty.")
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(name ?? String.Empty);
            }

            if (!parameterValue.Any())
            {
                throw new ArgumentException(isEmptyMessage ?? String.Empty, name ?? String.Empty);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if <paramref name="parameterValue"/> is null. 
        ///     In addition, if any of the contained instance properties with public getters are null, this method will
        ///     throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to check.</typeparam>
        /// <param name="parameterValue">The parameter to check for null alongside its properties.</param>
        /// <param name="name">The name of the string parameter. If null, <see cref="String.Empty"/> is used instead.</param>
        /// <param name="includeContainerNameWhenPropertyIsNull">A <see cref="bool"/> value expressing whether or not to include 
        ///     the <paramref name="name"/> value with a dot before the property name, such as "String.Empty" instead of just "Empty".</param>
        public static void AgainstNullDataContainer<T>(T parameterValue, string name, bool includeContainerNameWhenPropertyIsNull = true) where T : class
        {
            if (parameterValue == null)
            {
                throw new ArgumentNullException(name ?? String.Empty);
            }

            var properties = parameterValue.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => !p.PropertyType.IsValueType && p.CanRead);
            foreach (var property in properties)
            {
                var value = property.GetValue(parameterValue);
                if (value == null)
                {
                    var propertyName = (includeContainerNameWhenPropertyIsNull && name != null ? name + "." : String.Empty) + property.Name;
                    throw new ArgumentNullException(propertyName);
                }
            }
        }
    }

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
