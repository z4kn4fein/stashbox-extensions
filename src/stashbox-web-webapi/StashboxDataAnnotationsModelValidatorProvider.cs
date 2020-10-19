using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http.Metadata;
using System.Web.Http.Validation;
using System.Web.Http.Validation.Providers;
using System.Web.Http.Validation.Validators;

namespace Stashbox.Web.WebApi
{
    /// <summary>
    /// Represents the stashbox data annotations model validator provider.
    /// </summary>
    public class StashboxDataAnnotationsModelValidatorProvider : DataAnnotationsModelValidatorProvider
    {
        private readonly IDependencyResolver dependencyResolver;

        private readonly MethodInfo attributeGetter;

        /// <summary>
        /// Constructs a <see cref="StashboxDataAnnotationsModelValidatorProvider"/>
        /// </summary>
        /// <param name="dependencyResolver">The stashbox container instance.</param>
        public StashboxDataAnnotationsModelValidatorProvider(IDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
            this.attributeGetter = typeof(DataAnnotationsModelValidator).GetMethod("get_Attribute",
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        }

        /// <inheritdoc />
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<ModelValidatorProvider> validatorProviders, IEnumerable<Attribute> attributes)
        {
            var validators = base.GetValidators(metadata, validatorProviders, attributes).ToArray();
            foreach (var modelValidator in validators)
            {
                var attribute = this.attributeGetter.Invoke(modelValidator, new object[0]);
                this.dependencyResolver.BuildUp(attribute);
            }

            return validators;
        }
    }
}
