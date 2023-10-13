using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator;

internal sealed class RandomGeneratorGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.Random);
    public Type GrainInterface => typeof(IRandomGeneratorGrain);
}