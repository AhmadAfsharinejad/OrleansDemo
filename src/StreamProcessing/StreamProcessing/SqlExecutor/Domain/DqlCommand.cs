namespace StreamProcessing.SqlExecutor.Domain;

public record struct DqlCommand
{
    public string CommandText { get; set; }
    public IReadOnlyCollection<string> ParameterFileds { get; set; }
    public IReadOnlyCollection<DqlField> OutputFileds { get; set; }
}