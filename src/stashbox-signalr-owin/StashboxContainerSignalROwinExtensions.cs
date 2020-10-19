using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Stashbox.AspNet.SignalR;
using Stashbox.Utils;
using System;
using System.Reflection;

namespace Stashbox
{
    /// <summary>
    /// Holds the <see cref="IStashboxContainer"/> extension methods for SignalR integration.
    /// </summary>
    public static class AppBuilderSignalRExtensions
    {
        /// <summary>
        /// Adds the <see cref="IStashboxContainer"/> as the default dependency resolver and the default <see cref="IHubActivator"/>, also registers the available <see cref="IHub"/> and <see cref="PersistentConnection"/> implementations.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="config">The hub configuration.</param>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The container.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> or <paramref name="config"/> is <c>null</c>.
        /// </exception>
        public static IStashboxContainer AddOwinSignalR(this IStashboxContainer container, HubConfiguration config, params Assembly[] assemblies)
        {
            Shield.EnsureNotNull(container, nameof(container));
            Shield.EnsureNotNull(config, nameof(config));

            container.RegisterSingleton<Microsoft.AspNet.SignalR.IDependencyResolver, StashboxDependencyResolver>();
            container.RegisterSingleton<IHubActivator, StashboxHubActivator>();
            config.Resolver = container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>();

            return container.RegisterHubs(assemblies).RegisterPersistentConnections(assemblies);
        }

        /// <summary>
        /// Adds the <see cref="IStashboxContainer"/> as the default dependency resolver and the default <see cref="IHubActivator"/>, also registers the available <see cref="IHub"/> and <see cref="PersistentConnection"/> implementations.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="config">The hub configuration.</param>
        /// <param name="types">The types.</param>
        /// <returns>The container.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="container"/> or <paramref name="config"/> is <c>null</c>.
        /// </exception>
        public static IStashboxContainer AddOwinSignalRWithTypes(this IStashboxContainer container, HubConfiguration config, params Type[] types)
        {
            Shield.EnsureNotNull(container, nameof(container));
            Shield.EnsureNotNull(config, nameof(config));

            container.RegisterSingleton<Microsoft.AspNet.SignalR.IDependencyResolver, StashboxDependencyResolver>();
            container.RegisterSingleton<IHubActivator, StashboxHubActivator>();
            config.Resolver = container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>();

            return container.RegisterHubs(types).RegisterPersistentConnections(types);
        }
    }
}
