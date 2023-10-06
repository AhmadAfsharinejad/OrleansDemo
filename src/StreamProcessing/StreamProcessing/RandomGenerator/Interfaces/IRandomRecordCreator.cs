using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.RandomGenerator.Domain;

namespace StreamProcessing.RandomGenerator.Interfaces;

internal interface IRandomRecordCreator
{
    PluginRecord Create(Dictionary<string, RandomType> columnTypesByName);
}