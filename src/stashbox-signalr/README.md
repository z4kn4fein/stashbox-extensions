# stashbox-signalr [![Build status](https://ci.appveyor.com/api/projects/status/9uif84y4e5iwfyqy/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-signalr/branch/master) [![NuGet Version](https://buildstats.info/nuget/Stashbox.AspNet.SignalR)](https://www.nuget.org/packages/Stashbox.AspNet.SignalR/)
ASP.NET SignalR integration for Stashbox which provides dependency injection for SignalR hubs and persistent connections.

## Usage
```c#
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        var container = new StashboxContainer();
        //... configure container
        
        container.AddSignalR();
        //or
        container.AddSignalR(typeof(MyHub).Assembly);
    }
}
```
> The example above will set the `DependencyResolver` of the `GlobalHost` to the `StashboxDependencyResolver` and also registers a custom `IHubActivator` based on Stashbox. It will also register any `IHub` and `PersistentConnection` implementations found in the given assemblies.

## Customizations
You can also ask Stashbox to register your hubs and connections without setting the dependency resolver and hub activator:
```c#
public class MvcApplication : System.Web.HttpApplication
{
    protected void Application_Start()
    {
        var container = new StashboxContainer();
        //... configure container
        
        container.RegisterHubs(typeof(MyHub).Assembly);
        //or
        container.RegisterHubs(typeof(MyHub), typeof(MyHub2));
        
        container.RegisterPersistentConnections(typeof(MyConnection).Assembly);
        //or
        container.RegisterPersistentConnections(typeof(MyConnection), typeof(MyConnection2));
        
        //or register them directly
        container.Register<MyHub>(context => context.WithoutDisposalTracking());
        container.Register<MyConnection>(context => context.WithoutDisposalTracking());
    }
}
```
> Register you hubs, or the `IDisposable` dependencies of your hubs with the `.WithoutDisposalTracking()` option because SignalR handles their lifetime. 
