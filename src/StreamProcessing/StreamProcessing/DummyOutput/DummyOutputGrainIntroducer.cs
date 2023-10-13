using StreamProcessing.DummyOutput.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.DummyOutput;

internal sealed class DummyOutputGrainIntroducer : IPluginGrainIntroducer
{
    public PluginTypeId PluginTypeId => new(PluginTypeNames.DummyOutput);
    public Type GrainInterface => typeof(IDummyOutputGrain);
}