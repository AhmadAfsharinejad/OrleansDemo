using Microsoft.Extensions.DependencyInjection;

namespace StreamProcessing.Di;

public static class StreamDependencyInjection
{
    public static IServiceCollection AddStreamServices(this IServiceCollection collection)
    {
        var serviceAdders = FindDependencyIntroducers();
        foreach (var serviceAdder in serviceAdders)
        {
            serviceAdder.AddService(collection);
        }
        
        return collection;
    }
    
    private static IEnumerable<IServiceAdder> FindDependencyIntroducers()
    {
        var dependencyTypes = typeof(IServiceAdder).Assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           typeof(IServiceAdder).IsAssignableFrom(type))
            .ToArray();

        return dependencyTypes
            .Select(Activator.CreateInstance)
            .Cast<IServiceAdder>()
            .ToArray();
    }
}