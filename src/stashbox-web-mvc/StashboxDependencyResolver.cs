using System;
using System.Collections.Generic;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents the stashbox dependency resolver.
    /// </summary>
    public class StashboxDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly StashboxPerRequestScopeProvider scopeProvider;

        /// <summary>
        /// Constructs a stashbox dependency resolver.
        /// </summary>
        /// <param name="scopeProvider">The per request scope provider.</param>
        public StashboxDependencyResolver(StashboxPerRequestScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType) =>
                this.scopeProvider.GetOrCreateScope().Resolve(serviceType, nullResultAllowed: true);

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType) =>
                this.scopeProvider.GetOrCreateScope().ResolveAll(serviceType);
    }
}
