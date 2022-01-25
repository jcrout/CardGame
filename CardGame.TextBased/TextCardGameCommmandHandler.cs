using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public abstract class TextCardGameCommmandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected ITextInterface TextInterface { get; }

        public TextCardGameCommmandHandler(ITextInterface textInterface)
        {
            Guard.AgainstNull(textInterface, nameof(textInterface));

            this.TextInterface = textInterface;
        }

        public abstract void Handle(TCommand command);
    }
}