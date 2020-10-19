using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;

namespace Stashbox.Web.WebApi
{
    /// <summary>
    /// Represents the stashbox model validator provider.
    /// </summary>
    public class StashboxModelValidatorProvider : ModelValidatorProvider
    {
        private readonly IDependencyResolver dependencyResolver;
        private readonly IEnumerable<ModelValidatorProvider> modelValidatorProviders;

        /// <summary>
        /// Constructs a <see cref="StashboxModelValidatorProvider"/>
        /// </summary>
        /// <param name="dependencyResolver">The stashbox container instance.</param>
        /// <param name="modelValidatorProviders">The collection of the existing model validator providers.</param>
        public StashboxModelValidatorProvider(IDependencyResolver dependencyResolver, IEnumerable<ModelValidatorProvider> modelValidatorProviders)
        {
            Shield.EnsureNotNull(dependencyResolver, nameof(dependencyResolver));
            Shield.EnsureNotNull(modelValidatorProviders, nameof(modelValidatorProviders));

            this.dependencyResolver = dependencyResolver;
            this.modelValidatorProviders = modelValidatorProviders;
        }

        /// <inheritdoc />
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders)
        {
            var validators = this.modelValidatorProviders.SelectMany(provider => provider.GetValidators(metadata, validatorProviders)).ToList();
            foreach (var modelValidator in validators)
                this.dependencyResolver.BuildUp(modelValidator);

            return validators;
        }
    }
}
