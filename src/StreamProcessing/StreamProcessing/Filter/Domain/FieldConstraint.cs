using StreamProcessing.Filter.Interfaces;

namespace StreamProcessing.Filter.Domain;

public record struct FieldConstraint : IConstraint
{
    public string Key { get; set; }
    public object Value { get; set; }
    public ConstraintCondition Condition { get; set; }
}