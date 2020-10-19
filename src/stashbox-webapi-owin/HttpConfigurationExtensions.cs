using Microsoft.Owin;
using Owin;
using Stashbox;
using Stashbox.Utils;
using Stashbox.Web.WebApi;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Hosting;

namespace System.Web.Http
{
    /// <summary>
    /// Represents the owin web api related stashbox extensions.
    /// </summary>
    public static class AppBuilderOwinWebApiExtensions
    {
        private static readonly DelegatingHandler StashboxScopeHandler = new ScopeHandler();

        /// <summary>
        /// Sets a custom messagehandler that uses the owin per request scope configured by <see cref="AppBuilderExtensions.UseStashbox"/>.
        /// <para />
        /// Configures the <see cref="IStashboxContainer"/> by it's <see cref="StashboxContainerExtensions.AddWebApi"/> extension method.
        /// </summary>
        /// <param name="builder">The app builder.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="builder"/> or <paramref name="configuration"/> or <paramref name="container"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="AppBuilderExtensions.UseStashbox"/>
        /// <seealso cref="StashboxContainerExtensions.AddWebApi"/>
        public static IAppBuilder UseStashboxWebApi(this IAppBuilder builder, HttpConfiguration configuration, IStashboxContainer container)
        {
            Shield.EnsureNotNull(builder, nameof(builder));
            Shield.EnsureNotNull(configuration, nameof(configuration));
            Shield.EnsureNotNull(container, nameof(container));

            if (!configuration.MessageHandlers.Contains(StashboxScopeHandler))
                configuration.MessageHandlers.Insert(0, StashboxScopeHandler);

            container.AddWebApi(configuration);

            return builder;
        }

        /// <summary>
        /// Sets a custom messagehandler that uses the owin per request scope configured by <see cref="AppBuilderExtensions.UseStashbox"/>.
        /// </summary>
        /// <param name="builder">The app builder.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="builder"/> or <paramref name="configuration"/> is <c>null</c>.
        /// </exception>
        /// <seealso cref="AppBuilderExtensions.UseStashbox"/>
        public static IAppBuilder UseStashboxWebApiScopeHandler(this IAppBuilder builder, HttpConfiguration configuration)
        {
            Shield.EnsureNotNull(builder, nameof(builder));
            Shield.EnsureNotNull(configuration, nameof(configuration));

            if (!configuration.MessageHandlers.Contains(StashboxScopeHandler))
                configuration.MessageHandlers.Insert(0, StashboxScopeHandler);

            return builder;
        }

        private class ScopeHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var context = request.GetOwinContext();
                var scope = context?.GetCurrentStashboxScope();
                if (scope != null)
                    request.Properties[HttpPropertyKeys.DependencyScope] = new StashboxDependencyScope(scope);

                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
