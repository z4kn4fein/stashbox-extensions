using Microsoft.AspNet.SignalR.Hubs;

namespace Stashbox.AspNet.SignalR
{
    /// <summary>
    /// Represents a stashbox hub activator.
    /// </summary>
    public class StashboxHubActivator : IHubActivator
    {
        private readonly IDependencyResolver dependencyResolver;

        /// <summary>
        /// Constructs a <see cref="StashboxHubActivator"/>.
        /// </summary>
        /// <param name="dependencyResolver">The container.</param>
        public StashboxHubActivator(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        /// <inheritdoc />
        public IHub Create(HubDescriptor descriptor) => (IHub)this.dependencyResolver.Resolve(descriptor.HubType);
    }
}
