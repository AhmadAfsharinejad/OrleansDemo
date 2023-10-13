using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.RandomGenerator.Domain;

public record struct RandomColumn(StreamField Field, RandomType Type);
