using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Logic;

internal sealed class IntegerFilterService : DecimalFilterService, IGraterOrLessFilterService
{
    public override FieldType FieldType => FieldType.Integer;
}