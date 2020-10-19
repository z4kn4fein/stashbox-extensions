using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.AspNet.SignalR
{
    /// <summary>
    /// Represents a <see cref="DefaultDependencyResolver"/> using the <see cref="IStashboxContainer"/>.
    /// </summary>
    public class StashboxDependencyResolver : DefaultDependencyResolver
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxDependencyResolver"/>.
        /// </summary>
        /// <param name="dependencyResolver">The container.</param>
        public StashboxDependencyResolver(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public override object GetService(Type serviceType) =>
            this.dependencyResolver.Resolve(serviceType, nullResultAllowed: true) ?? base.GetService(serviceType);

        /// <inheritdoc />
        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var services = this.dependencyResolver.ResolveAll(serviceType);
            var baseServices = base.GetServices(serviceType);
            return baseServices == null ? services : services.Concat(baseServices);
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing) => this.dependencyResolver.Dispose();
    }
}
