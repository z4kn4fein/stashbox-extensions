using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Transports;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using SignalR.Tests.Common;
using Stashbox.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stashbox.SignalR.Tests
{
    [TestClass]
    public class SignalRExtensionTests
    {
        [TestMethod]
        public void ContainerExtensionTests_AddSignalR()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config);

            Assert.IsInstanceOfType(config.Resolver, typeof(StashboxDependencyResolver));
            Assert.IsTrue(container.CanResolve<Microsoft.AspNet.SignalR.IDependencyResolver>());
            Assert.IsTrue(container.CanResolve<IHubActivator>());
        }

        [TestMethod]
        public void ContainerExtensionTests_AddSignalR_WithAssembly()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config, typeof(TestHub).Assembly);

            Assert.IsTrue(container.CanResolve<TestHub>());
            Assert.IsTrue(container.CanResolve<Hub>());
            Assert.IsTrue(container.CanResolve<IHub>());
            Assert.IsTrue(container.CanResolve<TestConnection>());
            Assert.IsTrue(container.CanResolve<PersistentConnection>());
        }

        [TestMethod]
        public void ContainerExtensionTests_AddSignalR_WithTypes()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalRWithTypes(config, typeof(TestHub), typeof(TestConnection));

            Assert.IsTrue(container.CanResolve<TestHub>());
            Assert.IsTrue(container.CanResolve<Hub>());
            Assert.IsTrue(container.CanResolve<IHub>());
            Assert.IsTrue(container.CanResolve<TestConnection>());
            Assert.IsTrue(container.CanResolve<PersistentConnection>());
        }

        [TestMethod]
        public void DependencyResolverTests_GetService_Null()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config);

            Assert.IsNull(container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(ITest)));
        }

        [TestMethod]
        public void DependencyResolverTests_GetServices_Null()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config);

            Assert.IsTrue(!container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetServices(typeof(ITest)).Any());
        }

        [TestMethod]
        public void DependencyResolverTests_GetService_PreferContainer()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config);
            container.Register<ITest, Test>();
            container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().Register(typeof(ITest), () => new Test2());

            Assert.IsInstanceOfType(container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(ITest)), typeof(Test));
        }

        [TestMethod]
        public void DependencyResolverTests_GetServices_Concats_Container_And_DefaultResolver()
        {
            var config = new HubConfiguration();
            var container = new StashboxContainer().AddOwinSignalR(config);
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
            var config = new HubConfiguration();
            TestHub hub;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                container.AddOwinSignalR(config, typeof(TestHub).Assembly);
                hub = (TestHub)container.Resolve<Microsoft.AspNet.SignalR.IDependencyResolver>().GetService(typeof(TestHub));
            }

            Assert.IsFalse(hub.Disposed);
        }

        [TestMethod]
        public void HubActivatorTests_Create()
        {
            var config = new HubConfiguration();
            TestHub hub;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                container.AddOwinSignalR(config, typeof(TestHub).Assembly);
                hub = (TestHub)container.Resolve<IHubActivator>().Create(new HubDescriptor { HubType = typeof(TestHub) });
            }

            Assert.IsFalse(hub.Disposed);
        }

        [TestMethod]
        public async Task SignalRExtensionTests_Integration()
        {
            using (var host = TestServer.Create(app =>
            {
                var container = new StashboxContainer();
                container.Register<ITest, Test>();
                var config = new HubConfiguration();

                app.UseStashboxSignalRWithTypes(container, config, typeof(TestHub2));
                app.MapSignalR(config);
            }))
            using (var hubConnection = new HubConnection("http://test"))
            {
                var proxy = hubConnection.CreateHubProxy("TestHub2");

                var tcs = new TaskCompletionSource<string>();

                proxy.On("addMessage", data =>
                {
                    tcs.SetResult(data);
                });

                await hubConnection.Start(new AutoTransport(new MemoryHost(host.Handler)));

                await proxy.Invoke("Send", "hello");

                var result = await tcs.Task;

                Assert.AreEqual("hello", result);
            }
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

        public class TestHub2 : Hub
        {
            public TestHub2(ITest test)
            {

            }

            public Task Send(string message)
            {
                return this.Clients.All.addMessage(message);
            }
        }
    }
}
