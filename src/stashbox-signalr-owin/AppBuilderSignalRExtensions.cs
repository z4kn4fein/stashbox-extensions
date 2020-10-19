using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Stashbox;
using Stashbox.Utils;
using System;
using System.Reflection;

namespace Owin
{
    /// <summary>
    /// Holds the <see cref="IAppBuilder"/> extension methods for Stashbox SignalR integration.
    /// </summary>
    public static class AppBuilderSignalRExtensions
    {
        /// <summary>
        /// Adds the <see cref="IStashboxContainer"/> as the default dependency resolver and the default <see cref="IHubActivator"/>, also registers the available <see cref="IHub"/> and <see cref="PersistentConnection"/> implementations.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="container">The container.</param>
        /// <param name="config">The hub configuration.</param>
        /// <param name="assemblies">The assemblies to scan.</param>
        /// <returns>The container.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="app"/> or <paramref name="container"/> or <paramref name="config"/> is <c>null</c>.
        /// </exception>
        public static IAppBuilder UseStashboxSignalR(this IAppBuilder app, IStashboxContainer container, HubConfiguration config, params Assembly[] assemblies)
        {
            Shield.EnsureNotNull(app, nameof(app));

            container.AddOwinSignalR(config, assemblies);

            return app;
        }

        /// <summary>
        /// Adds the <see cref="IStashboxContainer"/> as the default dependency resolver and the default <see cref="IHubActivator"/>, also registers the available <see cref="IHub"/> and <see cref="PersistentConnection"/> implementations.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="container">The container.</param>
        /// <param name="config">The hub configuration.</param>
        /// <param name="types">The types.</param>
        /// <returns>The container.</returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="app"/> or <paramref name="container"/> or <paramref name="config"/> is <c>null</c>.
        /// </exception>
        public static IAppBuilder UseStashboxSignalRWithTypes(this IAppBuilder app, IStashboxContainer container, HubConfiguration config, params Type[] types)
        {
            Shield.EnsureNotNull(app, nameof(app));

            container.AddOwinSignalRWithTypes(config, types);

            return app;
        }
    }
}
