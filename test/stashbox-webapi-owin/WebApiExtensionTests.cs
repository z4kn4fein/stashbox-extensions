using Microsoft.Owin;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Stashbox.Web.WebApi;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Stashbox.AspNet.WebApi.Owin.Tests
{
    [TestClass]
    public class WebApiExtensionTests
    {
        [TestMethod]
        public async Task WebApiExtensionTests_Scoping()
        {
            using (var server = TestServer.Create(app =>
             {
                 var config = new HttpConfiguration { IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always };
                 var container = new StashboxContainer();
                 container.Register<TestMiddleware>();
                 container.RegisterScoped<Test>();

                 config.MapHttpAttributeRoutes();

                 app.UseStashboxWebApi(config, container).UseStashbox(container).UseWebApi(config);
             }))
            {
                var resp = await server.HttpClient.GetAsync("/api/test/value");
                Assert.AreEqual("test1\"1test1test1True\"", await resp.Content.ReadAsStringAsync());

                resp = await server.HttpClient.GetAsync("/api/test/value");
                Assert.AreEqual("test2\"2test2test2True\"", await resp.Content.ReadAsStringAsync());
            }
        }
    }

    public class Test
    {
        private static int counter;

        public Test()
        {
            Interlocked.Increment(ref counter);
        }

        public string Value => "test" + counter;
    }

    [RoutePrefix("api/test")]
    public class Test2Controller : ApiController
    {
        private readonly Test test;
        private readonly Test test1;
        private readonly IOwinContext context;
        private readonly IDependencyResolver resolver;

        private static int controllerCounter;

        public Test2Controller(Test test, Test test1, IOwinContext context, IDependencyResolver resolver)
        {
            this.test = test;
            this.test1 = test1;
            this.context = context;
            this.resolver = resolver;
            Interlocked.Increment(ref controllerCounter);
        }

        [HttpGet]
        [Route("value")]
        public IHttpActionResult GetValue()
        {
            var scope = (StashboxDependencyScope)this.Request.Properties[HttpPropertyKeys.DependencyScope];
            var fieldInfo = typeof(StashboxDependencyScope).GetField("dependencyResolver", BindingFlags.Instance | BindingFlags.NonPublic);
            var scopeResolver = fieldInfo.GetValue(scope);

            var ctx = this.Request.GetOwinContext();

            var scopeSame = this.resolver == this.context.GetCurrentStashboxScope() && this.resolver == scopeResolver && this.context == ctx;
            return this.Ok(controllerCounter + this.test.Value + this.test1.Value + scopeSame);
        }
    }

    public class TestMiddleware : OwinMiddleware
    {
        private readonly Test test;

        public TestMiddleware(OwinMiddleware next, Test test) : base(next)
        {
            this.test = test;
        }

        public override async Task Invoke(IOwinContext context)
        {
            await context.Response.WriteAsync(this.test.Value);
            await this.Next.Invoke(context);
        }
    }
}
