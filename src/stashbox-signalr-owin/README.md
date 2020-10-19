# stashbox-signalr-owin [![Build status](https://ci.appveyor.com/api/projects/status/nylw0crq5rhhx9k8/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-signalr-owin/branch/master) [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNet.SignalR.Owin)](https://www.nuget.org/packages/Stashbox.AspNet.SignalR.Owin/)
ASP.NET SignalR OWIN integration for Stashbox which provides the same functionality as the [standard SignalR integration](https://github.com/z4kn4fein/stashbox-signalr) but it contains `IAppBuilder` extensions and it sets the `HubConfiguration.Resolver` to the stashbox default dependency resolver instead of the `GlobalHost.DependencyResolver`.

## Usage
```c#
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        //configure container
        var container = new StashboxContainer();
        container.Register<IService1, Service1>();
        
        //register the custom hubs
        container.Register<CustomHub>(context => context.WithoutDisposalTracking());
        
        var config = new HubConfiguration();

        //register the default dependency resolver, hub activator, the same configuration as the standard integration package does
        app.UseStashboxSignalR(container, config);
        
        //enable signalr
        app.MapSignalR(config);
    }
}
```
