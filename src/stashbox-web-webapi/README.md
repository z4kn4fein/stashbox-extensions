# stashbox-web-webapi [![Build status](https://ci.appveyor.com/api/projects/status/3c4fv3c94f9cpfa1/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-web-webapi/branch/master) [![NuGet Version](https://buildstats.info/nuget/Stashbox.Web.WebApi)](https://www.nuget.org/packages/Stashbox.Web.WebApi/)
ASP.NET Web API integration for Stashbox which provides dependency injection for controllers, action filters and model validators.

## Usage
```c#
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        var container = new StashboxContainer();
        container.Register<IService1, Service1>();
        //...configure container
        
        container.AddWebApi(config);
    }
}
```

## Manual configuration
You can get more controll over the configuration by doing the steps manually and skip which you don't need:
```c#
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        var container = new StashboxContainer();
        container.Register<IService1, Service1>();
        //...configure container
        
        container.AddWebApiModelValidatorInjection(config);
        container.AddWebApiFilterProviderInjection(config);

        config.DependencyResolver = new StashboxDependencyResolver(container);
        container.RegisterWebApiControllers(config);
    }
}
```

## Owin
You can use this package with OWIN but it's recommended to use the [OWIN related web api package](https://github.com/z4kn4fein/stashbox-webapi-owin) instead.
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        var container = new StashboxContainer();
        container.Register<IService1, Service1>();
        //...configure container
    
        var httpConfiguration = new HttpConfiguration();
        //etc...

        container.AddWebApi(httpConfiguration);
        app.UseWebApi(httpConfiguration);
    }
}
```
