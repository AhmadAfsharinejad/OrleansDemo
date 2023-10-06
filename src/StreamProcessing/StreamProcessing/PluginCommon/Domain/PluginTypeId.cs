namespace StreamProcessing.PluginCommon.Domain;

using StronglyTypedIds;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
public partial struct PluginTypeId
{
    public PluginTypeId(){}

    public PluginTypeId(PluginTypeNames pluginTypeName)
    {
        Value = pluginTypeName.ToString();
    }
}