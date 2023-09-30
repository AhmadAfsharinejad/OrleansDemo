namespace HelloWorld.Grains.Domains.States;

[GenerateSerializer]
public record class EncryptionState
{
    [Id(0)]
    public string Id { get; set; }

    [Id(1)]
    public string Value { get; set; }
}