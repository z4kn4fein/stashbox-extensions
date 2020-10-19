using System;

namespace Stashbox.Owin
{
    internal static class Constants
    {
        public static string LifetimeScopeKey = "StashboxOwinLifetimeScope_" + Guid.NewGuid();
    }
}
