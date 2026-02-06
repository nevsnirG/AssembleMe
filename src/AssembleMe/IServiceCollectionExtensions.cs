using AssembleMe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAssembler(this IServiceCollection services) =>
        AddAssembler(services, null, AssemblerOptions.Default);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<AssemblerOptions>? configureOptions)
    {
        var options = new AssemblerOptions();
        configureOptions?.Invoke(options);
        return AddAssembler(services, null, options);
    }

    public static IServiceCollection AddAssembler(this IServiceCollection services, AssemblerOptions assemblerOptions) =>
        AddAssembler(services, null, assemblerOptions);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder>? configureBuilder) =>
        AddAssembler(services, configureBuilder, AssemblerOptions.Default);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder>? configureBuilder, Action<AssemblerOptions>? configureOptions)
    {
        var options = new AssemblerOptions();
        configureOptions?.Invoke(options);
        return AddAssembler(services, configureBuilder, options);
    }

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder>? configureBuilder, AssemblerOptions assemblerOptions)
    {
        if (configureBuilder is not null)
        {
            var assemblerBuilder = new AssemblerBuilder(services);
            configureBuilder(assemblerBuilder);
        }

        services.AddTransient<ICreateAssemblyProcessors, DependencyInjectedAssemblyProcessorCreator>();
        services.AddTransient<IAssembler>(sp => ActivatorUtilities.CreateInstance<Assembler>(sp, assemblerOptions));

        var serviceProvider = services.BuildServiceProvider();
        var assembler = serviceProvider.GetRequiredService<IAssembler>();

        assembler.Assemble();

        return services;
    }
}
