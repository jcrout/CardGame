namespace CardGame.Core
{
    public class PlayerCollectionFactory : IPlayerCollectionFactory
    {
        public IPlayerCollection Create()
        {
            return new PlayerCollection();
        }
    }
}