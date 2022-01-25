using System;

namespace CardGame.TextBased
{
    public interface ITextInterface
    {
        event EventHandler<KeyPressEventArgs> KeyPressed;

        string Title { get; set; }

        int BufferTop { get; set; }

        int BufferLeft { get; set; }

        int CursorLeft { get; set; }

        int CursorTop { get; set; }

        int WindowWidth { get; set; }

        int WindowHeight { get; set; }

        void Write(string text);

        void WriteLine(string text);

        void Clear();

        void Backspace();

        char ReadKey();

        string ReadLine();
    }
}