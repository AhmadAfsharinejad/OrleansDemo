using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.PluginCommon.Interfaces;

internal interface IStringReplacer
{
    string? Replace(string? contentTemplate, IReadOnlyCollection<string>? fields, PluginRecord record);
}