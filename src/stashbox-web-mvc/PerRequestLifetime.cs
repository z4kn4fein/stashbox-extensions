using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per web request lifetime.
    /// </summary>
    public class PerWebRequestLifetime : FactoryLifetimeDescriptor
    {
        /// <inheritdoc />
        protected override int LifeSpan { get; } = 10;

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory, ServiceRegistration serviceRegistration, ResolutionContext resolutionContext,
            Type resolveType) =>
            Constants.GetScopedValueMethod.MakeGenericMethod(resolveType)
                .CallStaticMethod(
                resolutionContext.CurrentScopeParameter, 
                factory.AsConstant(),
                resolutionContext.RequestContextParameter,
                serviceRegistration.RegistrationId.AsConstant(typeof(object)));

        private static TValue CollectScopedInstance<TValue>(IResolutionScope scope, Func<IResolutionScope, IRequestContext, object> factory, 
            IRequestContext requestContext, object scopeId)
            where TValue : class
        {
            if (HttpContext.Current == null)
                return null;

            if (HttpContext.Current.Items[scopeId] != null)
                return HttpContext.Current.Items[scopeId] as TValue;

            TValue instance;
            if (HttpContext.Current.Items[StashboxPerRequestScopeProvider.ScopeKey] is IResolutionScope requestScope)
            {
                instance = factory(requestScope, requestContext) as TValue;
                HttpContext.Current.Items[scopeId] = instance;
            }
            else
                instance = factory(scope, requestContext) as TValue;

            return instance;
        }
    }
}
