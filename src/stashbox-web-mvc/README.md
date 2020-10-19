# stashbox-web-mvc [![Build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions/main.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions/branch/main) [![NuGet Version](https://buildstats.info/nuget/Stashbox.Web.Mvc)](https://www.nuget.org/packages/Stashbox.Web.Mvc/)
ASP.NET MVC integration for [Stashbox](https://github.com/z4kn4fein/stashbox)

## Registering in Global.asax
```c#
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        //...
        
        StashboxConfig.RegisterStashbox(this.ConfigureServices);
    }

    private void ConfigureServices(IStashboxContainer container)
    {
        container.Register<IService1, Service1>();
        //etc...
    }
}
```
