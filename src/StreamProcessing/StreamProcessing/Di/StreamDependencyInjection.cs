using Microsoft.Extensions.DependencyInjection;

namespace StreamProcessing.Di;

public static class StreamDependencyInjection
{
    public static IServiceCollection AddStreamServices(this IServiceCollection collection)
    {
        var dependencyIntroducers = FindDependencyIntroducers();
        foreach (var dependencyIntroducer in dependencyIntroducers)
        {
            dependencyIntroducer.AddService(collection);
        }
        
        return collection;
    }
    
    private static IEnumerable<IDependencyIntroducer> FindDependencyIntroducers()
    {
        var dependencyInstallerTypes = typeof(IDependencyIntroducer).Assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           typeof(IDependencyIntroducer).IsAssignableFrom(type))
            .ToArray();

        return dependencyInstallerTypes
            .Select(Activator.CreateInstance)
            .Cast<IDependencyIntroducer>()
            .ToArray();
    }
}