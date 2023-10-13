namespace StreamProcessing.SqlExecutor.Domain;

public record struct DmlCommand
{
    public string CommandText { get; set; }
    public IReadOnlyCollection<string> ParameterFileds { get; set; }
}