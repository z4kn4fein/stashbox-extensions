using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Owin.Tests
{
    [TestClass]
    public class OwinExtensionTests
    {
        [TestMethod]
        public async Task OwinExtensionTests_Use_Scenario1()
        {
            var container = new StashboxContainer();
            var test = new Test { Content = "test" };
            container.RegisterInstance(test);
            container.Register<TestMiddleware>();
            container.Register<TestMiddleware2>();

            using (var server = TestServer.Create(app => app.UseStashbox(container)))
            {
                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("1test2", await response.Content.ReadAsStringAsync());
            }
        }

        [TestMethod]
        public async Task OwinExtensionTests_Use_Scenario2()
        {
            var container = new StashboxContainer();
            var test = new Test { Content = "test" };
            container.RegisterInstance(test);

            using (var server = TestServer.Create(app => app.UseStashbox(container)
                .UseViaStashbox<TestMiddleware>(container)
                .UseViaStashbox<TestMiddleware2>(container)))
            {
                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("1test2", await response.Content.ReadAsStringAsync());
            }
        }

        [TestMethod]
        public async Task OwinExtensionTests_Use_Scoping()
        {
            var container = new StashboxContainer(config => config.WithUnknownTypeResolution());
            container.RegisterScoped<Test1>();

            using (var server = TestServer.Create(app => app.UseStashbox(container)
                .UseViaStashbox<TestMiddleware3>(container)
                .UseViaStashbox<TestMiddleware2>(container)
                .Use<TestMiddleware>()))
            {
                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("121", await response.Content.ReadAsStringAsync());

                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("221", await response.Content.ReadAsStringAsync());
            }
        }

        [TestMethod]
        public async Task OwinExtensionTests_Context_Injection()
        {
            var container = new StashboxContainer(config => config.WithUnknownTypeResolution());

            using (var server = TestServer.Create(app => app.UseStashbox(container)
                .UseViaStashbox<TestMiddleware4>(container)))
            {
                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("TrueTrue", await response.Content.ReadAsStringAsync());

                using (var response = await server.HttpClient.GetAsync("/"))
                    Assert.AreEqual("TrueTrue", await response.Content.ReadAsStringAsync());
            }
        }

        public class Test
        {
            public string Content { get; set; }
        }

        public class Test1
        {
            public static int Counter;

            public Test1()
            {
                Interlocked.Increment(ref Counter);
            }
        }

        public class TestMiddleware : OwinMiddleware
        {
            public TestMiddleware(OwinMiddleware next) : base(next)
            {
            }

            public override async Task Invoke(IOwinContext context)
            {
                await context.Response.WriteAsync("1");
                await this.Next.Invoke(context);
            }
        }

        public class TestMiddleware2 : OwinMiddleware
        {
            private readonly Test test;

            public TestMiddleware2(OwinMiddleware next, Test test) : base(next)
            {
                this.test = test;
            }

            public override async Task Invoke(IOwinContext context)
            {
                await context.Response.WriteAsync(this.test.Content + "2");
                await this.Next.Invoke(context);
            }
        }

        public class TestMiddleware3 : OwinMiddleware
        {
            public TestMiddleware3(OwinMiddleware next, Test1 test) : base(next)
            {
            }

            public override async Task Invoke(IOwinContext context)
            {
                await context.Response.WriteAsync(Test1.Counter.ToString());
                await this.Next.Invoke(context);
            }
        }

        public class TestMiddleware4 : OwinMiddleware
        {
            private readonly IOwinContext ctx;
            private readonly IDependencyResolver resolver;

            public TestMiddleware4(OwinMiddleware next, IOwinContext ctx, IDependencyResolver resolver) : base(next)
            {
                this.ctx = ctx;
                this.resolver = resolver;
            }

            public override async Task Invoke(IOwinContext context)
            {
                var ctsSame = this.ctx == context;
                var scopeSame = this.resolver == this.ctx.GetCurrentStashboxScope();

                await context.Response.WriteAsync($"{ctsSame}{scopeSame}");
                await this.Next.Invoke(context);
            }
        }
    }
}
