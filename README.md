# AssembleMe

[![NuGet](https://img.shields.io/nuget/v/AssembleMe.svg)](https://www.nuget.org/packages/AssembleMe)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AssembleMe.svg)](https://www.nuget.org/packages/AssembleMe)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Minimal impact assembly discovery and scanning framework for .NET. Discover, scan and process assemblies from your filesystem or AppDomain with an easy plugin-like architecture.

## Why AssembleMe?

Many .NET applications need to discover and process assemblies at startup — for plugin systems, automatic service registration, convention-based configuration, or module loading. AssembleMe provides a clean, extensible way to do this with first-class dependency injection support.

- **Two discovery modes** — scan assemblies already loaded in the current `AppDomain`, or discover them from the filesystem (or both).
- **Plugin architecture** — implement `IProcessAssemblies` to define what happens with each discovered assembly. Register as many processors as you need.
- **DI-native** — integrates directly with `Microsoft.Extensions.DependencyInjection` via a single `AddAssembler()` call.
- **Abstractions package** — reference only `AssembleMe.Abstractions` in your libraries to avoid coupling to the implementation.

## Installation

```bash
# Main package (includes implementation + DI integration)
dotnet add package AssembleMe

# Abstractions only (for libraries that define processors)
dotnet add package AssembleMe.Abstractions
```

## Quick Start

```csharp
var serviceCollection = new ServiceCollection();

// Register one or more assembly processors
serviceCollection.AddTransient<IProcessAssemblies, MyAssemblyProcessor>();

// Configure and register the assembler
serviceCollection.AddAssembler(options =>
{
    options.AssembleAppDomainAssemblies = true;
    options.AssembleFileSystemAssemblies = true;
    options.AssembleFileSystemAssembliesRecursively = true;
    options.AssembleFromFileSystemDirectory = AppDomain.CurrentDomain.BaseDirectory;
});

var provider = serviceCollection.BuildServiceProvider();
var assembler = provider.GetRequiredService<IAssembler>();
assembler.Assemble();
```

## Core Concepts

### Assembly Processors

An assembly processor is any class that implements `IProcessAssemblies`. When `Assemble()` is called, each discovered assembly is passed to every registered processor. This is where you define your scanning logic — for example, finding types that implement a specific interface, applying attributes, or registering services.

```csharp
public class MyAssemblyProcessor : IProcessAssemblies
{
    public void Process(Assembly assembly)
    {
        // Example: find and register all types implementing IMyPlugin
        var pluginTypes = assembly.GetExportedTypes()
            .Where(t => typeof(IMyPlugin).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var type in pluginTypes)
        {
            // Do something with the discovered types
        }
    }
}
```

You can register multiple processors, and they will all run for each discovered assembly:

```csharp
services.AddTransient<IProcessAssemblies, PluginDiscoveryProcessor>();
services.AddTransient<IProcessAssemblies, AutoMapperProfileScanner>();
services.AddTransient<IProcessAssemblies, EventHandlerRegistrar>();
```

### Configuration Options

| Option | Type | Description |
|--------|------|-------------|
| `AssembleAppDomainAssemblies` | `bool` | Scan assemblies currently loaded in the `AppDomain`. |
| `AssembleFileSystemAssemblies` | `bool` | Scan `.dll` files from the filesystem. |
| `AssembleFileSystemAssembliesRecursively` | `bool` | When scanning the filesystem, include subdirectories. |
| `AssembleFromFileSystemDirectory` | `string` | Root directory for filesystem assembly discovery. |

### Package Structure

| Package | Purpose | Depends On |
|---------|---------|------------|
| [`AssembleMe`](https://www.nuget.org/packages/AssembleMe) | Implementation + DI registration | `AssembleMe.Abstractions`, `Microsoft.Extensions.DependencyInjection` |
| [`AssembleMe.Abstractions`](https://www.nuget.org/packages/AssembleMe.Abstractions) | Interfaces (`IAssembler`, `IProcessAssemblies`) | `Microsoft.Extensions.DependencyInjection.Abstractions` |

Reference `AssembleMe.Abstractions` in class libraries that define processors, and `AssembleMe` in your application entry point where you configure DI.

## Requirements

- .NET 8.0 or later

## Related Projects

- [AutoWire.MicrosoftDependencyInjection](https://www.nuget.org/packages/AutoWire.MicrosoftDependencyInjection) — automatic service registration built on AssembleMe

## License

This project is licensed under the [MIT License](LICENSE).
