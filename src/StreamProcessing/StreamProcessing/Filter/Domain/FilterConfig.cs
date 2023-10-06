using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.Filter.Domain;

[Immutable]
public record struct FilterConfig : IPluginConfig
{
    public IConstraint Constraint { get; set; }
}