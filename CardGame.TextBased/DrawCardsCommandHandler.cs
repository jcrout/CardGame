using CardGame.Core;
using CardGame.Utilities;

namespace CardGame.TextBased
{
    public class DrawCardsCommandHandler : TextCardGameCommmandHandler<DrawCardsCommand>
    {
        private ICardCollectionFactory cardCollectionFactory;
        private char drawCardKey;

        public DrawCardsCommandHandler(KeyInputSettings settings, ITextInterface textInterface, ICardCollectionFactory cardCollectionFactory) : base(textInterface)
        {
            Guard.AgainstNullDataContainer(settings, nameof(settings));
            Guard.AgainstNull(textInterface, nameof(textInterface));
            Guard.AgainstNull(cardCollectionFactory, nameof(cardCollectionFactory));

            this.drawCardKey = settings.DrawCardKey;
            this.cardCollectionFactory = cardCollectionFactory;
        }

        public override void Handle(DrawCardsCommand command)
        {
            Guard.AgainstNull(command, nameof(command));

            command.Deck.Shuffle();

            var cardsDrawnThisRound = this.cardCollectionFactory.Create();
            foreach (var player in command.Players)
            {
                this.TextInterface.Write($"{player.Name}, press {this.drawCardKey} to draw a card. ");
                this.TextInterface.ReadSingleCharFromList(new[] { this.drawCardKey });
                this.TextInterface.ClearCurrentLine();

                var drawnCard = command.Deck.Draw();
                var cardHasASuit = drawnCard.Suit != null;
                cardsDrawnThisRound.Add(drawnCard);

                this.TextInterface.Write($"{player.Name} drew ");

                if (cardHasASuit)
                {
                    this.TextInterface.Write("the ");
                    this.TextInterface.Write(drawnCard.FaceValue.Value);
                    this.TextInterface.Write($" of ");
                    this.TextInterface.Write(drawnCard.Suit.Name);
                    this.TextInterface.WriteLine(".");
                }
                else
                {
                    this.TextInterface.Write(StringUtility.AorAn(drawnCard.FaceValue.Value));
                    this.TextInterface.WriteLine(" card.");
                }
            }

            command.CardsDrawn = cardsDrawnThisRound;
        }
    }
}