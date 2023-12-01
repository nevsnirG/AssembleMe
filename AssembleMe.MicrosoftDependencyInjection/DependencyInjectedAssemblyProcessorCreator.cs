using AssembleMe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe.MicrosoftDependencyInjection;
internal sealed class DependencyInjectedAssemblyProcessorCreator : ICreateAssemblyProcessors
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectedAssemblyProcessorCreator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IProcessAssemblies Create(Type type)
    {
        return (ActivatorUtilities.CreateInstance(_serviceProvider, type) as IProcessAssemblies)!;
    }
}
