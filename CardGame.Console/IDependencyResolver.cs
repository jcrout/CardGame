namespace CardGame.Console
{
    /// <summary>
    ///     This interface is part of the composition root and is intended to be used with the <see cref="CardGameSettings"/> class to decouple it from the IoC Container.
    /// </summary>
    internal interface IDependencyResolver
    {
        T Get<T>();
    }
}