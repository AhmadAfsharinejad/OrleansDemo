using StreamProcessing.Filter.Interfaces;

namespace StreamProcessing.Filter.Domain;

public record struct LogicalConstraint : IConstraint
{
    public IReadOnlyCollection<IConstraint> Constraints { get; set; }
    public ConstraintOperator Operator { get; set; }
}