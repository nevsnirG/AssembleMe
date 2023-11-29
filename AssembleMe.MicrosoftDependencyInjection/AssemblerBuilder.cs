using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe.MicrosoftDependencyInjection;

public interface IAssemblerBuilder
{
    IServiceCollection Services { get; }
}

public sealed record AssemblerBuilder(IServiceCollection Services) : IAssemblerBuilder;
