using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request lifetime.
    /// </summary>
    public class PerRequestLifetime : FactoryLifetimeDescriptor
    {
        /// <inheritdoc />
        protected override int LifeSpan { get; } = 10;

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, object> factory, ServiceRegistration serviceRegistration, ResolutionContext resolutionContext,
            Type resolveType) =>
            Constants.GetScopedValueMethod.MakeGenericMethod(resolveType)
                .CallStaticMethod(resolutionContext.CurrentScopeParameter, factory.AsConstant(), serviceRegistration.RegistrationId.AsConstant(typeof(object)));

        private static TValue CollectScopedInstance<TValue>(IResolutionScope scope, Func<IResolutionScope, object> factory, object scopeId)
            where TValue : class
        {
            if (HttpContext.Current == null)
                return null;

            if (HttpContext.Current.Items[scopeId] != null)
                return HttpContext.Current.Items[scopeId] as TValue;

            TValue instance;
            if (HttpContext.Current.Items[StashboxPerRequestScopeProvider.ScopeKey] is IResolutionScope requestScope)
            {
                instance = factory(requestScope) as TValue;
                HttpContext.Current.Items[scopeId] = instance;
            }
            else
                instance = factory(scope) as TValue;

            return instance;
        }
    }
}
