using AssembleMe.Abstractions;

namespace AssembleMe;
public sealed class DefaultAssemblyProcessorCreator : ICreateAssemblyProcessors
{
    public IProcessAssemblies Create(Type type)
    {
        var instance = (Activator.CreateInstance(type, true) as IProcessAssemblies)!;
        return instance;
    }
}
