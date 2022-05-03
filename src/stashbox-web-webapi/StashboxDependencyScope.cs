using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Stashbox.Web.WebApi
{
    /// <summary>
    /// Represents a stashbox dependency scope.
    /// </summary>
    public class StashboxDependencyScope : IDependencyScope
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxDependencyScope"/>.
        /// </summary>
        /// <param name="dependencyResolver">The container.</param>
        public StashboxDependencyScope(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public object GetService(Type serviceType) => this.dependencyResolver.ResolveOrDefault(serviceType);

        /// <inheritdoc />
        public IEnumerable<object> GetServices(Type serviceType) => this.dependencyResolver.ResolveAll(serviceType);

        /// <inheritdoc />
        public void Dispose() => this.dependencyResolver.Dispose();
    }
}
