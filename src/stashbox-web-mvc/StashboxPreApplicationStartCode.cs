using System.Threading;
using System.Web;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Stashbox.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(StashboxPreApplicationStartCode), "Start")]

namespace Stashbox.Web.Mvc
{
    /// <summary>
    /// Holds a pre application start method which can be used with the <see cref="PreApplicationStartMethodAttribute"/>.
    /// </summary>
    public static class StashboxPreApplicationStartCode
    {
        private static int isStarted;

        /// <summary>
        /// The pre application start method.
        /// </summary>
        public static void Start()
        {
            if (Interlocked.CompareExchange(ref isStarted, 1, 0) == 0)
                DynamicModuleUtility.RegisterModule(typeof(StashboxPerRequestLifetimeHttpModule));
        }
    }
}
