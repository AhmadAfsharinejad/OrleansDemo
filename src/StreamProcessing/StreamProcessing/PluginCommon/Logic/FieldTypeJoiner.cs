using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;

namespace StreamProcessing.PluginCommon.Logic;

internal sealed class FieldTypeJoiner : IFieldTypeJoiner
{
    public IReadOnlyDictionary<string, FieldType> Join(IReadOnlyDictionary<string, FieldType>? inputFieldTypesByName,
        IEnumerable<StreamField>? pluginFields,
        RecordJoinType joinType)
    {
        return joinType switch
        {
            RecordJoinType.InputOnly => inputFieldTypesByName ?? throw new InvalidOperationException("Input can't be null."),
            RecordJoinType.ResultOnly => Convert(pluginFields),
            RecordJoinType.Append => Append(inputFieldTypesByName, pluginFields),
            _ => throw new ArgumentOutOfRangeException(nameof(joinType), joinType, null)
        };
    }
    
    private static IReadOnlyDictionary<string, FieldType> Convert(IEnumerable<StreamField>? pluginFields)
    {
        if (pluginFields is null)
        {
            throw new InvalidOperationException("Plugin output fields can't be null.");
        }

        return pluginFields.ToDictionary(x => x.Name, y => y.Type);
    }

    private static IReadOnlyDictionary<string, FieldType> Append(IReadOnlyDictionary<string, FieldType>? inputFieldTypesByName,
        IEnumerable<StreamField>? pluginFields)
    {
        if (inputFieldTypesByName is null && pluginFields is null)
        {
            throw new InvalidOperationException("Both input type and plugin fields type can't be null.");
        }

        var result = inputFieldTypesByName is null ? new Dictionary<string, FieldType>() : new Dictionary<string, FieldType>(inputFieldTypesByName);

        if (pluginFields is null)
        {
            return result;
        }

        foreach (var pluginField in pluginFields)
        {
            result.Add(pluginField.Name, pluginField.Type);
        }

        return result;
    }
}