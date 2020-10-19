using System.Reflection;

namespace Stashbox.Web.Mvc
{
    internal static class Constants
    {
        internal static MethodInfo GetScopedValueMethod = typeof(PerRequestLifetime).GetTypeInfo().GetDeclaredMethod("CollectScopedInstance");
    }
}
