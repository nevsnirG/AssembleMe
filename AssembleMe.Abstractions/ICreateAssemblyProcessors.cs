namespace AssembleMe.Abstractions;
public interface ICreateAssemblyProcessors
{
    IProcessAssemblies Create(Type type);
}
