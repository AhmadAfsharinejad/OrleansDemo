using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest.Logic;

internal sealed class RestOutputFieldTypeGetter : IRestOutputFieldTypeGetter
{
    private readonly IFieldTypeJoiner _fieldTypeJoiner;

    public RestOutputFieldTypeGetter(IFieldTypeJoiner fieldTypeJoiner)
    {
        _fieldTypeJoiner = fieldTypeJoiner ?? throw new ArgumentNullException(nameof(fieldTypeJoiner));
    }

    public IReadOnlyDictionary<string, FieldType> GetOutputs(PluginExecutionContext pluginContext, RestConfig config)
    {
        var outputs = new List<StreamField>();

        if (config.ResponseHeaders is not null)
        {
            foreach (var requestHeader in config.ResponseHeaders)
            {
                outputs.Add(new StreamField(requestHeader.FieldName, FieldType.Text));
            }
        }

        if (!string.IsNullOrWhiteSpace(config.StatusFieldName))
        {
            outputs.Add(new StreamField(config.StatusFieldName, FieldType.Integer));
        }
        
        if (!string.IsNullOrWhiteSpace(config.ResponseContentFieldName))
        {
            outputs.Add(new StreamField(config.ResponseContentFieldName, FieldType.Text));
        }

        return _fieldTypeJoiner.Join(pluginContext.InputFieldTypes, outputs, config.JoinType);
    }
}