using CardGame.Utilities;
using Ninject;

namespace CardGame.Console
{
    /// <summary>
    ///     Ninject implementation of the <see cref="IDependencyResolver"/> interface.
    /// </summary>
    internal class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            Guard.AgainstNull(kernel, nameof(kernel));

            this.kernel = kernel;
        }

        public T Get<T>()
        {
            return this.kernel.Get<T>();
        }
    }
}