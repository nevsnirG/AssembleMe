using AssembleMe.Abstractions;
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
    private readonly List<string> _discoveredAssemblyFullNames;
    private readonly List<IProcessAssemblies> _assemblyProcessors;
    private readonly ICreateAssemblyProcessors _assemblyProcessorCreator;

#pragma warning disable IDE1006 // Naming Styles
    private static readonly IEnumerable<IProcessAssemblies> NoProcessors = Enumerable.Empty<IProcessAssemblies>();
#pragma warning restore IDE1006 // Naming Styles

    public Assembler() : this(NoProcessors, new DefaultAssemblyProcessorCreator(), AssemblerOptions.Default) { }

    public Assembler(AssemblerOptions options) : this(NoProcessors, new DefaultAssemblyProcessorCreator(), options) { }

    public Assembler(IEnumerable<IProcessAssemblies> assemblyProcessors) : this(assemblyProcessors, AssemblerOptions.Default) { }

    public Assembler(IEnumerable<IProcessAssemblies> assemblyProcessors, AssemblerOptions options) : this(assemblyProcessors, new DefaultAssemblyProcessorCreator(), options) { }

    public Assembler(IEnumerable<IProcessAssemblies> assemblyProcessors, ICreateAssemblyProcessors assemblyProcessorCreator, AssemblerOptions options)
    {
        ArgumentNullException.ThrowIfNull(assemblyProcessors);
        ArgumentNullException.ThrowIfNull(assemblyProcessorCreator);
        ArgumentNullException.ThrowIfNull(options);

        _options = options;
        _assemblyProcessors = assemblyProcessors.ToList();
        _assemblyProcessorCreator = assemblyProcessorCreator;
        _discoveredAssemblyFullNames = new();
    }

    public void Assemble()
    {
        if (_options.AssembleAppDomainAssemblies)
            ScanAppDomainAssemblies();

        if (_options.AssembleFileSystemAssemblies)
            ScanAssembliesFromDirectory();

        ProcessAssemblies();
    }

    private void ScanAppDomainAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            DiscoverAssembly(assembly);
        }
    }

    private void ScanAssembliesFromDirectory()
    {
        var context = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());

        var directory = new DirectoryInfo(_options.AssembleFromFileSystemDirectory);
        var searchOptions = _options.AssembleFileSystemAssembliesRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var file in directory.EnumerateFiles("*.dll", searchOptions))
        {
            if (!context!.Assemblies.Any(a => a.Location.Equals(file.FullName, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (TryLoadAssemblyFromFile(context, file.FullName, out var assembly))
                {
                    DiscoverAssembly(assembly);
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

    private void DiscoverAssembly(Assembly assembly)
    {
        if (string.IsNullOrEmpty(assembly.FullName))
            return;

        var assemblyFullName = assembly.FullName.ToLowerInvariant();
        if (_discoveredAssemblyFullNames.Contains(assemblyFullName))
            return;

        if (_options.DiscoverAssemblyProcessors)
            DiscoverIn(assembly);

        _discoveredAssemblyFullNames.Add(assemblyFullName);
    }

    public void DiscoverIn(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (IsAssemblyProcessor(type) && AssemblyProcessorAlreadyDiscovered(type))
            {
                var instance = _assemblyProcessorCreator.Create(type);
                _assemblyProcessors.Add(instance);
            }
        }
    }

    private static bool IsAssemblyProcessor(Type type)
    {
        return !type.IsInterface && !type.IsAbstract && type.IsAssignableTo(typeof(IProcessAssemblies));
    }

    private bool AssemblyProcessorAlreadyDiscovered(Type type)
    {
        return !_assemblyProcessors.Any(pdp => pdp.GetType() == type);
    }

    private void ProcessAssemblies()
    {
        foreach (var assemblyFullName in _discoveredAssemblyFullNames)
        {
            var assembly = Assembly.Load(assemblyFullName);
            foreach (var assemblyProcessor in _assemblyProcessors)
            {
                assemblyProcessor.ProcessAssembly(assembly);
            }
        }
    }
}
