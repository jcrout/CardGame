using System;
using System.Linq;
using System.Text;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public static class Extensions
    {
        public static void WriteLine(this ITextInterface @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            @this.WriteLine(String.Empty);
        }

        public static int ReadIntegerInRange(this ITextInterface @this, int minimum, int maximum)
        {
            Guard.AgainstNull(@this, nameof(@this));

            if (maximum < minimum)
            {
                throw new ArgumentException($"{nameof(minimum)} must be less than or equal to {nameof(maximum)}.");
            }

            int value = 0;
            int startIndex = @this.CursorLeft;
            var input = "";

            while (true)
            {
                var inputChar = @this.ReadKey();
                if (char.IsNumber(inputChar))
                {
                    input += inputChar;
                    var yoyo = (int)inputChar;
                    value = Int32.Parse(input);
                    if (value >= minimum)
                    {
                        if (value <= maximum)
                        {
                            return value;
                        }
                        else
                        {
                            input = input.Substring(0, input.Length - 1);
                            @this.Backspace();
                        }
                    }
                }
                else if (inputChar == 8)
                {
                    if (@this.CursorLeft == startIndex - 1)
                    {
                        @this.Write(" ");
                    }
                    else
                    {
                        input = (input.Length > 0) ? input.Substring(0, input.Length - 1) : input;
                        @this.Write(" \b");
                    }
                }
                else
                {
                    @this.Backspace();
                }
            }
        }

        public static char ReadSingleCharFromList(this ITextInterface @this, params char[] validChars)
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNullOrEmptyEnumerable(validChars, nameof(validChars));

            var upperValidChars = validChars.Select(chr => Char.IsLower(chr) ? Char.ToUpper(chr) : chr);
            while (true)
            {
                var inputChar = @this.ReadKey();
                if (char.IsLower(inputChar))
                {
                    inputChar = char.ToUpper(inputChar);
                }

                if (upperValidChars.Contains(inputChar))
                {
                    return inputChar;
                }
                else
                {
                    @this.Backspace();
                }
            }
        }

        public static bool GetYesOrNoResponse(this ITextInterface @this, string questionText, char yesChar = 'Y', char noChar = 'N')
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNull(questionText, nameof(questionText));

            @this.Write($"{questionText} ({yesChar}|{noChar}): ");
            return ReadSingleCharFromList(@this, new char[] { yesChar, noChar }) == yesChar;
        }

        public static void ClearCurrentLine(this ITextInterface @this)
        {
            Guard.AgainstNull(@this, nameof(@this));

            @this.SetLine(@this.CursorTop, new string(' ', @this.WindowWidth));
            @this.CursorLeft = @this.BufferLeft;
        }

        public static string ReadLine(this ITextInterface @this, int maximumLength, int minimumLength = 0)
        {
            Guard.AgainstNull(@this, nameof(@this));

            var builder = new StringBuilder();

            while (true)
            {
                var inputKey = @this.ReadKey();

                if (inputKey == 13)
                {
                    if (builder.Length >= minimumLength)
                    {
                        break;
                    }
                    else
                    {
                        @this.Backspace();
                    }
                }
                else if (inputKey == 8)
                {
                    if (builder.Length > 0)
                    {
                        builder.Remove(builder.Length - 1, 1);
                    }
                    else
                    {
                        @this.Write(" ");
                    }
                }
                else if (builder.Length < maximumLength)
                {
                    builder.Append(inputKey);
                }
                else
                {
                    @this.Backspace();
                }
            }

            return builder.ToString();
        }

        public static void SetLine(this ITextInterface @this, int lineIndex, string text)
        {
            Guard.AgainstNull(@this, nameof(@this));
            Guard.AgainstNull(text, nameof(text));

            if (text.Length > @this.WindowWidth)
            {
                throw new ArgumentException($"{nameof(text)} must be less than {nameof(ITextInterface)}.{nameof(@this.WindowWidth)}");
            }

            int currentLine = @this.CursorTop;
            int currentColumn = @this.CursorLeft;

            @this.CursorTop = lineIndex;
            @this.CursorLeft = 0;

            @this.Write(text);

            if (text.Length < @this.WindowWidth)
            {
                for (int i = text.Length; i < @this.WindowWidth; i++)
                {
                    @this.Write(" ");
                }
            }

            @this.CursorTop = currentLine;
            @this.CursorLeft = currentColumn;
        }
    }
}