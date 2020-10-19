# stashbox-owin [![Build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-extensions/main.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions/branch/master) [![NuGet Version](https://buildstats.info/nuget/Stashbox.Owin)](https://www.nuget.org/packages/Stashbox.Owin/)
OWIN integration for Stashbox which provides dependency injection and scope management for your custom OWIN middlewares.

## Usage
The registration order of the middlewares is very **important**, that order will be used to add them to the OWIN pipeline.
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        var container = new StashboxContainer();
        //... configure container, register dependencies
        
        //Register the middlewares that will use dependency injection
        container.Register<CustomMiddleware>();
        container.Register<CustomMiddleware2>();
        
        app.UseStashbox(container);
        
        //Register the other middlewares
    }
}
```
> The `app.UseStashbox(container)` method will insert a scope handler middleware as the first item in the pipeline and after that the middlewares that are registered into Stashbox.

## Custom order
There is an option to register your middleware wherever you want into the pipeline (but after the scope middleware) by using the `UseViaStashbox<TMiddleware>` method:
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        var container = new StashboxContainer();
        //... configure container, register dependencies
        
        //Register the scope middleware only
        app.UseStashbox(container);
        
        app.Use<SimpleMiddleware1>();
        app.UseViaStashbox<DependencyInjectedMiddleware1>(container);
        app.Use<SimpleMiddleware2>();
        app.Use<SimpleMiddleware3>();
        app.UseViaStashbox<DependencyInjectedMiddleware2>(container)
    }
}
```
