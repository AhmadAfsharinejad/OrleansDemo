using StreamProcessing.Filter.Interfaces;

namespace StreamProcessing.Filter.Domain;

public record struct FieldConstraint : IConstraint
{
    public string FieldName { get; set; }
    public object Value { get; set; }
    public ConstraintOperators Operator { get; set; }
}