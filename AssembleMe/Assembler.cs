using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace AssembleMe;
public interface IAssembler
{
    void Assemble();
}
public sealed class Assembler : IAssembler
{
    private readonly AssemblerOptions _options;
    private readonly List<string> _processedAssemblies;
    private readonly IEnumerable<IProcessAssemblies> _assemblyProcessors;

    public Assembler(AssemblerOptions options, IEnumerable<IProcessAssemblies> assemblyProcessors)
    {
        _options = options;
        _processedAssemblies = new();
        _assemblyProcessors = assemblyProcessors;
    }

    public void Assemble()
    {
        if (_options.AssembleAppDomainAssemblies)
            ScanAppDomainAssemblies();

        if (_options.AssembleFileSystemAssemblies)
            ScanAssembliesFromDirectory();
    }

    private void ScanAppDomainAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            ProcessAssembly(assembly);
        }
    }

    private void ScanAssembliesFromDirectory()
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());

        var directory = new DirectoryInfo(_options.AssembleFromFileSystemDirectory);
        var searchOptions = SearchOption.AllDirectories;
        foreach (var file in directory.EnumerateFiles("*.dll", searchOptions))
        {
            if (!context!.Assemblies.Any(a => a.Location.Equals(file.FullName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (TryLoadAssemblyFromFile(context, file.FullName, out var assembly))
                {
                    ProcessAssembly(assembly);
                }
            }
        }
    }

    private static bool TryLoadAssemblyFromFile(AssemblyLoadContext context, string fileName, [NotNullWhen(true)] out Assembly? assembly)
    {
        try
        {
            assembly = context.LoadFromAssemblyPath(fileName);
        }
        catch (Exception ex) when (ex is FileLoadException or FileNotFoundException or BadImageFormatException)
        {
            assembly = null;
            return false;
        }

        return true;
    }

    private void ProcessAssembly(Assembly assembly)
    {
        var assemblyFullName = assembly.FullName!.ToLowerInvariant();
        if (_processedAssemblies.Contains(assemblyFullName))
            return;

        foreach (var assemblyProcessor in _assemblyProcessors)
            assemblyProcessor.ProcessAssembly(assembly);

        _processedAssemblies.Add(assemblyFullName);
    }
}
