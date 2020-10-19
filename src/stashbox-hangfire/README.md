# stashbox-hangfire 
[![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox-hangfire/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox-hangfire/branch/master) [![Travis CI build status](https://img.shields.io/travis/com/z4kn4fein/stashbox-hangfire/master.svg?label=travis-ci)](https://travis-ci.com/z4kn4fein/stashbox-hangfire) [![NuGet Version](https://buildstats.info/nuget/Hangfire.Stashbox)](https://www.nuget.org/packages/Hangfire.Stashbox/)

This project provides [Stashbox](https://github.com/z4kn4fein/stashbox) integration for [Hangfire](https://www.hangfire.io/), using Stashbox container and Scopes to resolve jobs and their dependencies.

## Common usage
To integrate Stashbox as the default `JobActivator` into Hangfire, you can use the `UseStashboxActivator` extension method on the `IGlobalConfiguration` interface.
```c#
var container = new StashboxContainer();

GlobalConfiguration.Configuration.UseStashboxActivator(container);
```

## ASP.NET Core
The ASP.NET Core extension of Hangfire uses the built-in `IServiceProvider` to resolve jobs through an `AspNetCoreJobActivator`, so for using Stashbox as the default job activator you can simply just use the [ASP.NET Core integration of Stashbox](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection) which replaces the default `IServiceProvider` with a Stashbox container. 

However you also have the option to tell Hangfire that it should use the `StashboxJobActivator` directly.
```c#
public IServiceProvider ConfigureService(IServiceCollection services)
{
    services.AddHangfire((provider, config) => 
        config.UseStashboxActivator(provider.GetService<IDependencyResolver>()));

    return services.UseStashbox();
}
```

## .NET Generic Host
```c#
using (var host = new HostBuilder()
    .UseStashbox()
    .ConfigureContainer<IStashboxContainer>((context, container) =>
    {
        container.Register<JobActivator, StashboxJobActivator>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHangfireServer();
    })
    .Build())
{
    // start and use your host
}
```
