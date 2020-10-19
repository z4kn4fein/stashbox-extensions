using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request scope provider using the <see cref="StashboxContainer"/>.
    /// </summary>
    public class StashboxPerRequestScopeProvider
    {
        /// <summary>
        /// The scope identifier.
        /// </summary>
        public const string ScopeKey = "stashboxRequestScope";

        private readonly IStashboxContainer container;

        /// <summary>
        /// Constructs a per request scope provider.
        /// </summary>
        /// <param name="container">The stashbox container.</param>
        public StashboxPerRequestScopeProvider(IStashboxContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Gets or creates a scope.
        /// </summary>
        /// <returns>The scope.</returns>
        public IDependencyResolver GetOrCreateScope()
        {
            var scope = HttpContext.Current?.Items[ScopeKey] as IDependencyResolver;

            if (scope == null && HttpContext.Current != null)
                HttpContext.Current.Items[ScopeKey] = scope = this.container.BeginScope();

            return scope;
        }

        /// <summary>
        /// Closes the current per request scope.
        /// </summary>
        public static void TerminateScope()
        {
            var scope = HttpContext.Current?.Items[ScopeKey] as IDependencyResolver;
            scope?.Dispose();
        }
    }
}
