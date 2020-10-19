using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Stashbox.Web.WebApi
{
    /// <summary>
    /// Represents the stashbox filter attribute filter provider.
    /// </summary>
    public class StashboxFilterProvider : IFilterProvider
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IEnumerable<IFilterProvider> filterProviders;

        /// <summary>
        /// Constructs a <see cref="StashboxFilterProvider"/>
        /// </summary>
        /// <param name="dependencyResolver">The stashbox container instance.</param>
        /// <param name="filterProviders">The collection of the existing filter providers.</param>
        public StashboxFilterProvider(IDependencyResolver dependencyResolver, IEnumerable<IFilterProvider> filterProviders)
        {
            Shield.EnsureNotNull(dependencyResolver, nameof(dependencyResolver));
            Shield.EnsureNotNull(filterProviders, nameof(filterProviders));

            this.dependencyResolver = dependencyResolver;
            this.filterProviders = filterProviders;
        }

        /// <inheritdoc />
        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var filters = this.filterProviders.SelectMany(provider => provider.GetFilters(configuration, actionDescriptor)).ToArray();
            foreach (var filter in filters)
                this.dependencyResolver.BuildUp(filter.Instance);

            return filters;
        }
    }
}
