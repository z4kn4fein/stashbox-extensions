using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Stashbox.Web.Mvc
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
            Shield.EnsureNotNull(dependencyResolver, nameof(dependencyResolver));

            this.dependencyResolver = dependencyResolver;
            this.attributeGetter = typeof(DataAnnotationsModelValidator).GetMethod("get_Attribute",
                BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        }

        /// <inheritdoc />
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            var validators = base.GetValidators(metadata, context, attributes).ToArray();
            foreach (var modelValidator in validators)
            {
                var attribute = this.attributeGetter.Invoke(modelValidator, new object[0]);
                this.dependencyResolver.BuildUp(attribute);
            }

            return validators;
        }
    }
}
