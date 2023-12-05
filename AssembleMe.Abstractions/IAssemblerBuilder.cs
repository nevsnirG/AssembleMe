using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe.Abstractions;

public interface IAssemblerBuilder
{
    IServiceCollection Services { get; }
}
