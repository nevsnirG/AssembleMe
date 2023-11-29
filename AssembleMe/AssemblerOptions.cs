namespace AssembleMe;
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
    /// If <see cref="AssembleFileSystemAssemblies"> is set to true, only assemblies in the top directory are scanned.
    /// </summary>
    public bool AssembleFileSystemAssembliesRecursively { get; set; } = true;
    public string AssembleFromFileSystemDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;

    public static AssemblerOptions Default => new();
}
