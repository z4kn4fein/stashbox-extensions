using System.Web;

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Represents a per request lifetime module.
    /// </summary>
    public class StashboxPerRequestLifetimeHttpModule : IHttpModule
    {
        /// <inheritdoc />
        public void Init(HttpApplication context)
        {
            context.EndRequest += (sender, e) => StashboxPerRequestScopeProvider.TerminateScope();
        }

        /// <inheritdoc />
        public void Dispose()
        { }
    }
}
