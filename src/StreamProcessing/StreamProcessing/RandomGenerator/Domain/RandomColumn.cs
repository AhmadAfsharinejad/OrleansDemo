using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.RandomGenerator.Domain;

public record struct RandomColumn(string Name, RandomType Type, FieldType FieldType);
