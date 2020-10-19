using Shouldly;
using Stashbox;
using System;
using Xunit;

namespace Hangfire.Stashbox.Tests
{
    public class StashboxActivatorTests
    {
        [Fact]
        public void Activate()
        {
            var activator = new StashboxJobActivator(new StashboxContainer().Register<TestJob>());
            var job = activator.ActivateJob(typeof(TestJob));
            job.ShouldNotBeNull();
        }

        [Fact]
        public void ActivateWithDependency()
        {
            var activator = new StashboxJobActivator(new StashboxContainer()
                .Register<TestJobWithDependency>()
                .Register<Dependency>());
            var job = activator.ActivateJob(typeof(TestJobWithDependency)) as TestJobWithDependency;
            job.ShouldNotBeNull();
            job.Dependency.ShouldNotBeNull();
        }

        [Fact]
        public void ActivateWithScope()
        {
            var activator = new StashboxJobActivator(new StashboxContainer()
                .Register<TestJobWithDependency>()
                .RegisterScoped<Dependency>());

            TestJobWithDependency job;
            using (var scope = activator.BeginScope((JobActivatorContext)null))
            {
                job = scope.Resolve(typeof(TestJobWithDependency)) as TestJobWithDependency;
                job.Dependency.Disposed.ShouldBeFalse();
            }

            job.Dependency.Disposed.ShouldBeTrue();
        }
    }

    class TestJob
    { }

    class TestJobWithDependency
    {
        public Dependency Dependency { get; }

        public TestJobWithDependency(Dependency dependency)
        {
            Dependency = dependency;
        }
    }

    class Dependency : IDisposable
    {
        public bool Disposed { get; set; }

        public void Dispose()
        {
            this.Disposed = true;
        }
    }
}
