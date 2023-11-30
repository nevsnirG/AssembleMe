# AssembleMe
Minimal impact assembly discovery and scanning framework. Discover, scan and process assemblies from your filesystem or AppDomain with an easy plugin-like architecture.

## Usage
Manually instantiate and run the assembler.
```csharp
var options = new AssemblerOptions
{
    AssembleAppDomainAssemblies = true,
    AssembleFileSystemAssemblies = true,
    AssembleFileSystemAssembliesRecursively = true,
    AssembleFromFileSystemDirectory = AppDomain.CurrentDomain.BaseDirectory
};
var assembler = new Assembler(new[] { new AssemblyProcessorImpl() }, options);
assembler.Assemble();
```

### AssembleMe.MicrosoftDependencyInjection
This library adds support for resolving assembly processors from the dependency container. Do note that the order of registration is important; the assembler is run on the call to AddAssembler.
```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<IProcessAssemblies, AssemblyProcessorImpl>();
serviceCollection.AddAssembler(AssemblerOptions.Default);
```
