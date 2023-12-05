using AssembleMe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AssembleMe;
internal sealed record AssemblerBuilder(IServiceCollection Services) : IAssemblerBuilder;