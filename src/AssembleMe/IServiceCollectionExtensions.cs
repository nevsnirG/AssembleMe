using AssembleMe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAssembler(this IServiceCollection services) =>
        AddAssembler(services, null, AssemblerOptions.Default);

    public static IServiceCollection AddAssembler(this IServiceCollection services, AssemblerOptions assemblerOptions) =>
        AddAssembler(services, null, assemblerOptions);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder>? configure) =>
        AddAssembler(services, configure, AssemblerOptions.Default);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder>? configure, AssemblerOptions assemblerOptions)
    {
        if (configure is not null)
        {
            var assemblerBuilder = new AssemblerBuilder(services);
            configure(assemblerBuilder);
        }

        services.AddTransient<ICreateAssemblyProcessors, DependencyInjectedAssemblyProcessorCreator>();
        services.AddTransient<IAssembler>(sp => ActivatorUtilities.CreateInstance<Assembler>(sp, assemblerOptions));

        var serviceProvider = services.BuildServiceProvider();
        var assembler = serviceProvider.GetRequiredService<IAssembler>();

        assembler.Assemble();

        return services;
    }
}
