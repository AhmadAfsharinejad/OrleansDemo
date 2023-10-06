﻿using Orleans.Concurrency;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IPluginGrain : IGrainWithGuidKey
{
    [ReadOnly]
    //[OneWay]
    Task Compute(Guid scenarioId, Guid pluginId, Immutable<PluginRecords>? pluginRecords, GrainCancellationToken cancellationToken);
}