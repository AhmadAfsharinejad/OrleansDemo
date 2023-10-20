using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.PluginCommon.Logic;

internal sealed class StringReplacer : IStringReplacer
{
    public string? Replace(string? contentTemplate, IReadOnlyCollection<string>? contentFields, PluginRecord record)
    {
        if (string.IsNullOrWhiteSpace(contentTemplate)) return null;

        if (contentFields is null || contentFields.Count == 0) return contentTemplate;

        var args = new object[contentFields.Count];
        var index = 0;
        foreach (var contentField in contentFields)
        {
            args[index++] = record.Record[contentField];
        }

        return string.Format(contentTemplate, args);
    }
}