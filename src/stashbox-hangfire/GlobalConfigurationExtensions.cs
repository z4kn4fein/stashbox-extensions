using Hangfire.Stashbox;
using Stashbox;
using Stashbox.Utils;
using System;

namespace Hangfire
{
    /// <summary>
    /// Extension methods for the <see cref="IGlobalConfiguration"/>.
    /// </summary>
    public static class GlobalConfigurationExtensions
    {
        /// <summary>
        /// Configures the given <see cref="IDependencyResolver"/> as the default <see cref="JobActivator"/> for resolving job dependencies.
        /// </summary>
        /// <param name="configuration">The global configuration.</param>
        /// <param name="container">The dependency resolver.</param>
        /// <returns>The <see cref="IGlobalConfiguration{StashboxJobActivator}"/> instance which uses the given Stashbox container to resolve job dependencies.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="container"/> is null.</exception>
        public static IGlobalConfiguration<StashboxJobActivator> UseStashboxActivator(
            this IGlobalConfiguration configuration,
            IDependencyResolver container)
        {
            Shield.EnsureNotNull(container, nameof(container));

            return configuration.UseActivator(new StashboxJobActivator(container));
        }
    }
}
