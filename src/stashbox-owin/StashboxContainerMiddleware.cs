using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Stashbox.Utils;

namespace Stashbox.Owin
{
    internal class StashboxContainerMiddleware<TMiddleware> : OwinMiddleware
        where TMiddleware : OwinMiddleware
    {
        public StashboxContainerMiddleware(OwinMiddleware next) : base(next)
        { }

        public override async Task Invoke(IOwinContext context)
        {
            var scope = context.GetCurrentStashboxScope();

            Shield.EnsureNotNull(scope, "Stashbox lifetime scope.");

            var middleware = scope.ResolveOrDefault<Func<OwinMiddleware, TMiddleware>>();
            if (middleware == null)
                await base.Next.Invoke(context);
            else
                await middleware(base.Next).Invoke(context);
        }
    }
}