namespace CardGame.TextBased
{
    public class KeyPressEventArgs : System.EventArgs
    {
        public char KeyPressed { get; }

        public bool Handled { get; set; } = false;

        public KeyPressEventArgs(char keyPressed)
        {
            this.KeyPressed = keyPressed;
        }
    }
}