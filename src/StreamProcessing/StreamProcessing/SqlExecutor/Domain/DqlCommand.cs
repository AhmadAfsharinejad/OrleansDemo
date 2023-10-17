namespace StreamProcessing.SqlExecutor.Domain;

public record struct DqlCommand
{
    public string CommandText { get; set; }
    public IReadOnlyCollection<string> ParameterFields { get; set; }
    public IReadOnlyCollection<DqlField> OutputFields { get; set; }
}