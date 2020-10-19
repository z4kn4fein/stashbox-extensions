using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Stashbox.Web.WebApi.Tests
{
    public class TestFilterAttribute : ActionFilterAttribute
    {
        [Dependency]
        public Test Test { get; set; }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            actionExecutedContext.Response.Headers.Add("test", this.Test.Value);
        }
    }

    public class TestValidationAttribute : ValidationAttribute
    {
        [Dependency]
        public Test Test { get; set; }

        public override bool IsValid(object value) => this.Test != null && !string.IsNullOrWhiteSpace(value.ToString());
    }

    [RoutePrefix("api/test")]
    public class TestController : ApiController
    {
        private readonly Test2 test2;

        public TestController(Test2 test2)
        {
            this.test2 = test2;
        }

        [HttpGet]
        [Route("value")]
        [TestFilter]
        public IHttpActionResult GetValue()
        {
            return this.Ok(this.test2.Value);
        }

        [HttpPost]
        [Route("value")]
        public IHttpActionResult PostValue(TestRequest request)
        {
            if (request == null)
                return this.BadRequest();

            return this.Ok();
        }
    }

    [RoutePrefix("api/test2")]
    public class Test2Controller : ApiController
    {
        private readonly Test3 test;
        private readonly Test3 test1;

        private static int controllerCounter;

        public Test2Controller(Test3 test, Test3 test1)
        {
            this.test = test;
            this.test1 = test1;
            Interlocked.Increment(ref controllerCounter);
        }

        [HttpGet]
        [Route("value")]
        public IHttpActionResult GetValue()
        {
            return this.Ok(controllerCounter + this.test.Value + this.test1.Value);
        }
    }

    public class TestRequest
    {
        [TestValidation]
        public string Sample { get; set; }
    }

    public class Test
    {
        public string Value => "test";
    }

    public class Test2
    {
        public string Value => "test2";
    }

    public class Test3
    {
        private static int counter;

        public Test3()
        {
            Interlocked.Increment(ref counter);
        }

        public string Value => "test" + counter;
    }

    [TestClass]
    public class WebApiTests
    {
        [TestMethod]
        public async Task Controller_ActionFilterTest()
        {
            using (var config = new HttpConfiguration())
            {
                var container = new StashboxContainer();
                config.MapHttpAttributeRoutes();
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                container.Register<Test>().Register<Test2>();
                container.AddWebApi(config);

                using (var server = new HttpServer(config))
                using (var client = new HttpClient(server))
                {
                    using (var response = await client.GetAsync("http://fakeurl/api/test/value"))
                    {
                        response.EnsureSuccessStatusCode();
                        Assert.IsTrue(response.Headers.Contains("test"));
                        Assert.AreEqual("test", response.Headers.GetValues("test").First());
                        Assert.AreEqual("\"test2\"", await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }

        [TestMethod]
        public async Task Controller_ValidationTest()
        {
            using (var config = new HttpConfiguration())
            {
                var container = new StashboxContainer();
                config.MapHttpAttributeRoutes();
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                container.Register<Test>().Register<Test2>();
                container.AddWebApi(config);

                using (var server = new HttpServer(config))
                using (var client = new HttpClient(server))
                {
                    var content = new ObjectContent<TestRequest>(new TestRequest { Sample = "test" },
                        new JsonMediaTypeFormatter());
                    using (var response = await client.PostAsync("http://fakeurl/api/test/value", content))
                        response.EnsureSuccessStatusCode();
                }
            }
        }

        [TestMethod]
        public async Task Controller_ScopedTest()
        {
            using (var config = new HttpConfiguration())
            {
                var container = new StashboxContainer();
                config.MapHttpAttributeRoutes();
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

                container.RegisterScoped<Test3>();
                container.AddWebApi(config);

                using (var server = new HttpServer(config))
                using (var client = new HttpClient(server))
                {
                    using (var response = await client.GetAsync("http://fakeurl/api/test2/value"))
                    {
                        response.EnsureSuccessStatusCode();
                        Assert.AreEqual("\"1test1test1\"", await response.Content.ReadAsStringAsync());
                    }

                    using (var response = await client.GetAsync("http://fakeurl/api/test2/value"))
                    {
                        response.EnsureSuccessStatusCode();
                        Assert.AreEqual("\"2test2test2\"", await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }
    }
}
