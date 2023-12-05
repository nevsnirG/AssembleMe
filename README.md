# AssembleMe
Minimal impact assembly discovery and scanning framework. Discover, scan and process assemblies from your filesystem or AppDomain with an easy plugin-like architecture.

## Usage
Registering the assembler in the Microsoft.Extensions.DependencyInjection container.
```csharp
var options = new AssemblerOptions
{
    AssembleAppDomainAssemblies = true,
    AssembleFileSystemAssemblies = true,
    AssembleFileSystemAssembliesRecursively = true,
    AssembleFromFileSystemDirectory = AppDomain.CurrentDomain.BaseDirectory
};
var serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<IProcessAssemblies, AssemblyProcessorImpl>();
serviceCollection.AddAssembler(AssemblerOptions.Default);
var assembler = serviceCollection.BuildServiceProvider().GetRequiredService<IAssembler>();
assembler.Assemble();
```