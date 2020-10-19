using Stashbox;
using System;

namespace Hangfire.Stashbox
{
    internal class StashboxJobActivatorScope : JobActivatorScope
    {
        private readonly IDependencyResolver scope;

        public StashboxJobActivatorScope(IDependencyResolver scope)
        {
            this.scope = scope;
        }

        public override object Resolve(Type type) => this.scope.Resolve(type);

        public override void DisposeScope() => this.scope.Dispose();
    }
}
