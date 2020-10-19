using Stashbox;
using Stashbox.Owin;

namespace Microsoft.Owin
{
    /// <summary>
    /// Represents the stashbox related <see cref="IOwinContext"/> extension methods.
    /// </summary>
    public static class OwinContextExtensions
    {
        /// <summary>
        /// Gets the current lifetime scope stored in the current <see cref="IOwinContext"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The scope.</returns>
        public static IDependencyResolver GetCurrentStashboxScope(this IOwinContext context) =>
            context.Get<IDependencyResolver>(Constants.LifetimeScopeKey);
    }
}
