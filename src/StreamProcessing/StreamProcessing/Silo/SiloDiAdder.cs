using Microsoft.Extensions.DependencyInjection;
using StreamProcessing.Di;
using StreamProcessing.Silo.Interfaces;

namespace StreamProcessing.Silo;

internal sealed class SiloDiAdder : IServiceAdder
{
    public void AddService(IServiceCollection collection)
    {
        collection.AddSingleton<IEachSiloCaller, EachSiloCaller>();
    }
}