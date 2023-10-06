namespace StreamProcessing.Scenario.Domain;

public record struct LinkConfig
{
    public Guid SourceId { get; set; }
    public Guid DestinationId { get; set; }
}