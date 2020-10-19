using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.AspNet.SignalR;
using System;
using System.Linq;

namespace Stashbox.SignalR.Tests
{
    [TestClass]
    public class SignalRExtensionTests
    {
        [TestMethod]
        public void ContainerExtensionTests_AddSignalR()
        {
            var container = new StashboxContainer().AddSignalR();

            Assert.IsInstanceOfType(GlobalHost.DependencyResolver, typeof(StashboxDependencyResolver));
            Assert.IsTrue(container.CanResolve<Microsoft.AspNet.SignalR.IDependencyResolver>());
            Assert.IsTrue(container.CanResolve<IHubActivator>());
        }

        [TestMethod]
        public void ContainerExtensionTests_AddSignalR_WithAssembly()
        {
            var container = new StashboxContainer().AddSignalR(typeof(TestHub).Assembly);
            Assert.IsTrue(container.CanResolve<TestHub>());
            Assert.IsTrue(container.CanResolve<Hub>());
            Assert.IsTrue(container.CanResolve<IHub>());
            Assert.IsTrue(container.CanResolve<TestConnection>());
            Assert.IsTrue(container.CanResolve<PersistentConnection>());
        }

        [TestMethod]
        public void ContainerExtensionTests_AddSignalR_WithTypes()
        {
            var container = new StashboxContainer().AddSignalRWithTypes(typeof(TestHub), typeof(TestConnection));
            Assert.IsTrue(container.CanResolve<TestHub>());
            Assert.IsTrue(container.CanResolve<Hub>());
            Assert.IsTrue(container.CanResolve<IHub>());
            Assert.IsTrue(container.CanResolve<TestConnection>());
            Assert.IsTrue(container.CanResolve<PersistentConnection>());
        }

        [TestMethod]
        public void DependencyResolverTests_GetService_Null()
        {
            var container = new StashboxContainer().AddSignalR();

            Assert.IsNull(container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(ITest)));
        }

        [TestMethod]
        public void DependencyResolverTests_GetServices_Null()
        {
            var container = new StashboxContainer().AddSignalR();

            Assert.IsTrue(!container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetServices(typeof(ITest)).Any());
        }

        [TestMethod]
        public void DependencyResolverTests_GetService_PreferContainer()
        {
            var container = new StashboxContainer().AddSignalR();
            container.Register<ITest, Test>();
            container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().Register(typeof(ITest), () => new Test2());

            Assert.IsInstanceOfType(container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(ITest)), typeof(Test));
        }

        [TestMethod]
        public void DependencyResolverTests_GetServices_Concats_Container_And_DefaultResolver()
        {
            var container = new StashboxContainer().AddSignalR();
            container.Register<ITest, Test>();
            container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().Register(typeof(ITest), () => new Test2());

            var services = container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetServices(typeof(ITest));

            Assert.AreEqual(2, services.Count());
            Assert.IsInstanceOfType(services.First(), typeof(Test));
            Assert.IsInstanceOfType(services.Last(), typeof(Test2));
        }

        [TestMethod]
        public void DependencyResolverTests_GetService_NotDisposing()
        {
            TestHub hub;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                container.AddSignalR(typeof(TestHub).Assembly);
                hub = (TestHub)container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(TestHub));
            }

            Assert.IsFalse(hub.Disposed);
        }

        [TestMethod]
        public void HubActivatorTests_Create()
        {
            TestHub hub;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                container.AddSignalR(typeof(TestHub).Assembly);
                hub = (TestHub)container.Resolve<IHubActivator>().Create(new HubDescriptor { HubType = typeof(TestHub) });
            }

            Assert.IsFalse(hub.Disposed);
        }

        public interface ITest
        { }

        public class Test : ITest
        { }

        public class Test2 : ITest
        { }

        public class TestHub : Hub
        {
            public ITest Test { get; }

            public TestHub(ITest test)
            {
                this.Test = test;
            }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                if (this.Disposed)
                    throw new ObjectDisposedException("Test hub disposed.");

                this.Disposed = true;
                base.Dispose(disposing);
            }
        }

        public class TestConnection : PersistentConnection
        {
            public ITest Test { get; }

            public TestConnection(ITest test)
            {
                this.Test = test;
            }
        }
    }
}
