using Microsoft.Extensions.DependencyInjection;

namespace StreamProcessing.Di;

internal interface IServiceAdder
{
    void AddService(IServiceCollection collection);
}