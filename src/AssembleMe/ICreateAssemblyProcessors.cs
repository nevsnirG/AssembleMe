using AssembleMe.Abstractions;

namespace AssembleMe;
public interface ICreateAssemblyProcessors
{
    IProcessAssemblies Create(Type type);
}
