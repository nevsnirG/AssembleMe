using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe.MicrosoftDependencyInjection;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder> configure) =>
        AddAssembler(services, configure, AssemblerOptions.Default);

    public static IServiceCollection AddAssembler(this IServiceCollection services, Action<IAssemblerBuilder> configure, AssemblerOptions assemblerOptions)
    {
        if (configure is null)
            return services;

        var assemblerBuilder = new AssemblerBuilder(services);
        configure(assemblerBuilder);

        services.AddTransient(sp => new Assembler(assemblerOptions, sp.GetServices<IProcessAssemblies>()));

        var serviceProvider = services.BuildServiceProvider();
        var assembler = serviceProvider.GetRequiredService<IAssembler>();

        assembler.Assemble();

        return services;
    }
}
