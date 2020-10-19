# stashbox-webapi-owin [![Build status](https://ci.appveyor.com/api/projects/status/hvaetoxf69cj9g2s/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-webapi-owin/branch/master) [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNet.WebApi.Owin)](https://www.nuget.org/packages/Stashbox.AspNet.WebApi.Owin/)
ASP.NET Web API OWIN integration for Stashbox which provides the same functionality as the [standard WebApi integration](https://github.com/z4kn4fein/stashbox-web-webapi) but with the scope management of the [OWIN package](https://github.com/z4kn4fein/stashbox-owin).

## Usage
You can use the lifetime support which comes from the Stashbox.Owin package, it can be useful when you have custom middlewares also and you'd like to use the same scoped dependencies as your controllers using within the same scope.
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        //configure container
        var container = new StashboxContainer();
        container.RegisterScoped<IService1, Service1>();
        
        var config = new HttpConfiguration();
        
        //register the scope middleware
        app.UseStashbox(container)
        
        //register scope message handler, controllers, etc..
        app.UseStashboxWebApi(config, container);
        
        //configure the WebApi
        app.UseWebApi(config);
    }
}
```

## Without the OWIN scope support
If you don't want to use the scope middleware, you can let the WebApi just use the `IDependencyResolver` implementation provided by the standard WebApi Stashbox integration.
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        //configure container
        var container = new StashboxContainer();
        container.RegisterScoped<IService1, Service1>();
        
        var config = new HttpConfiguration();
        
        //register controllers, the dependency resolver, custom providers just like in the standard integration package
        container.AddWebApi(config);
        
        //configure the WebApi
        app.UseWebApi(config);
    }
}
```
