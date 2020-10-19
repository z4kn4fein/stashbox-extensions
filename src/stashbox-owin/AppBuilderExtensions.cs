using Microsoft.Owin;
using Stashbox;
using Stashbox.Owin;
using Stashbox.Utils;
using System;
using System.Linq;

namespace Owin
{
    /// <summary>
    /// Represents the stashbox related <see cref="IAppBuilder"/> extension methods.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Registers a new resolution scope into the owin pipeline and stores it in the current <see cref="IOwinContext"/>, also adds all the registered Owin middlewares into the <see cref="IAppBuilder"/>.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="container">The container.</param>
        /// <returns>The app builder.</returns>
        /// <example>
        /// <code>
        /// var container = new StashboxConatiner();
        /// container.RegisterType&lt;CustomMiddleware&gt;();
        /// app.UseStashbox(container);
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="app"/> or <paramref name="container"/> is <c>null</c>.
        /// </exception>
        public static IAppBuilder UseStashbox(this IAppBuilder app, IStashboxContainer container)
        {
            Shield.EnsureNotNull(app, nameof(app));
            Shield.EnsureNotNull(container, nameof(container));

            return app.Use<StashboxScopeMiddleware>(container)
                      .WireRegisteredMiddlewares(container);
        }

        /// <summary>
        /// Registers an owin middleware into the pipeline through stashbox.
        /// </summary>
        /// <param name="app">The app builder.</param>
        /// <param name="container">The container.</param>
        /// <returns>The app builder.</returns>
        /// <example>
        /// <code>
        /// var container = new StashboxConatiner();
        /// app.UseStashbox(container);
        /// app.UseViaStashbox&lt;CustomMiddleware&gt;();
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="app"/> or <paramref name="container"/> is <c>null</c>.
        /// </exception>
        public static IAppBuilder UseViaStashbox<TMiddleware>(this IAppBuilder app, IStashboxContainer container)
            where TMiddleware : OwinMiddleware
        {
            Shield.EnsureNotNull(app, nameof(app));
            Shield.EnsureNotNull(container, nameof(container));

            container.Register<TMiddleware>();
            return app.Use<StashboxContainerMiddleware<TMiddleware>>();
        }

        private static IAppBuilder WireRegisteredMiddlewares(this IAppBuilder app, IStashboxContainer container)
        {
            var middlewares = container.GetRegistrationMappings()
                .Where(reg => typeof(OwinMiddleware).IsAssignableFrom(reg.Value.ImplementationType))
                .OrderBy(reg => reg.Value.RegistrationId)
                .Select(reg => typeof(StashboxContainerMiddleware<>).MakeGenericType(reg.Value.ImplementationType));

            foreach (var middleware in middlewares)
                app.Use(middleware);

            return app;
        }
    }
}
