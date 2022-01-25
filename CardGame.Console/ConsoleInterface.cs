using System;
using System.Text;
using CardGame.TextBased;

namespace CardGame.Console
{
    /// <summary>
    ///     Adapter class implementing <see cref="ITextInterface"/> for the static Console class.
    /// </summary>
    public class ConsoleInterface : ITextInterface
    {
        public event EventHandler<KeyPressEventArgs> KeyPressed;

        private int bufferTop = 0;
        private int bufferLeft = 1;
        private int lastLeft = 0;
        private int lastTop = 0;

        public ConsoleInterface()
        {
            this.WriteLeftBuffer();
            this.lastLeft = System.Console.CursorLeft;
            this.lastTop = System.Console.CursorTop;
        }

        public int BufferTop
        {
            get
            {
                return this.bufferTop;
            }

            set
            {
                this.bufferTop = value;
            }
        }

        public int BufferLeft
        {
            get
            {
                return this.bufferLeft;
            }

            set
            {
                this.bufferLeft = value;
                this.WriteLeftBuffer();
            }
        }

        public int CursorLeft
        {
            get
            {
                return System.Console.CursorLeft;
            }

            set
            {
                System.Console.CursorLeft = value >= this.bufferLeft ? value : this.bufferLeft;
            }
        }

        public int CursorTop
        {
            get
            {
                return System.Console.CursorTop;
            }

            set
            {
                System.Console.CursorTop = value;
            }
        }

        public int WindowWidth
        {
            get
            {
                return System.Console.WindowWidth;
            }

            set
            {
                System.Console.WindowWidth = value;
            }
        }

        public int WindowHeight
        {
            get
            {
                return System.Console.WindowHeight;
            }

            set
            {
                System.Console.WindowHeight = value;
            }
        }

        public string Title
        {
            get
            {
                return System.Console.Title;
            }

            set
            {
                System.Console.Title = value ?? String.Empty;
            }
        }

        public void Backspace()
        {
            if (System.Console.CursorTop != this.lastTop)
            {
                System.Console.CursorTop = this.lastTop;
                System.Console.CursorLeft = this.lastLeft;
            }
            else
            {
                this.Write("\b \b");
            }
        }

        public char ReadKey()
        {
            this.lastLeft = System.Console.CursorLeft;
            this.lastTop = System.Console.CursorTop;

            var consoleKeyInfo = System.Console.ReadKey(true);
            var keyPressed = consoleKeyInfo.KeyChar;

            if (!HandleKeyPress(consoleKeyInfo))
            {
                if (consoleKeyInfo.KeyChar == 13)
                {
                    System.Console.WriteLine();
                }
                else if (consoleKeyInfo.KeyChar == 8)
                {
                    this.Backspace();
                }
                else
                {
                    System.Console.Write(consoleKeyInfo.KeyChar);
                }
            }

            return keyPressed;
        }

        private bool HandleKeyPress(ConsoleKeyInfo consoleKeyInfo)
        {
            var keyEvent = this.KeyPressed;
            if (keyEvent != null)
            {
                var args = new KeyPressEventArgs(consoleKeyInfo.KeyChar);
                keyEvent(this, args);

                return args.Handled;
            }
            else
            {
                return false;
            }
        }

        public string ReadLine()
        {
            var builder = new StringBuilder();
            while (true)
            {
                var key = this.ReadKey();
                if (key == 13)
                {
                    break;
                }
                else
                {
                    builder.Append(key);
                }
            }

            return builder.ToString();
        }

        private void WriteLeftBuffer()
        {
            int index = System.Console.CursorLeft;
            for (int i = index; i < this.BufferLeft; i++)
            {
                System.Console.Write(' ');
            }
        }

        public void Write(string text)
        {
            if (System.Console.CursorLeft < this.BufferLeft)
            {
                WriteLeftBuffer();
            }

            System.Console.Write(text);
        }

        public void WriteLine(string text)
        {
            this.Write(text);
            System.Console.WriteLine();
            WriteLeftBuffer();
        }

        public void Clear()
        {
            System.Console.Clear();
        }
    }
}