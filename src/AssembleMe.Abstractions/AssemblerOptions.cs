namespace AssembleMe.Abstractions;
public sealed class AssemblerOptions
{
    /// <summary>
    /// Scan all assemblies in the current <see cref="AppDomain"/>.
    /// </summary>
    public bool AssembleAppDomainAssemblies { get; set; } = true;
    /// <summary>
    /// Scan the file system for assemblies.
    /// </summary>
    public bool AssembleFileSystemAssemblies { get; set; } = true;
    /// <summary>
    /// If <see cref="AssembleFileSystemAssemblies" /> is set to true, assemblies from this directory are scanned.
    /// </summary>
    public string AssembleFromFileSystemDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
    /// <summary>
    /// If <see cref="AssembleFileSystemAssemblies" /> is set to true, only assemblies in the top directory are scanned.
    /// </summary>
    public bool AssembleFileSystemAssembliesRecursively { get; set; } = true;
    /// <summary>
    /// Discover <see cref="IProcessAssemblies" /> implementations from the scanned assemblies.
    /// </summary>
    public bool DiscoverAssemblyProcessors { get; set; } = true;

    public static AssemblerOptions Default => new();
}
