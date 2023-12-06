using System.Reflection;

namespace AssembleMe.Abstractions;
public interface IProcessAssemblies
{
    void ProcessAssembly(Assembly assembly);
}
