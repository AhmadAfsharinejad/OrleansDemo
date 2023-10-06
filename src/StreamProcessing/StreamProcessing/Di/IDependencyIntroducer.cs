using Microsoft.Extensions.DependencyInjection;

namespace StreamProcessing.Di;

internal interface IDependencyIntroducer
{
    void AddService(IServiceCollection collection);
}