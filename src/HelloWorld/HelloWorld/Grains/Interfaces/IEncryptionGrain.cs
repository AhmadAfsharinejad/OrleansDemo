namespace HelloWorld.Grains.Interfaces;

public interface IEncryptionGrain : IGrainWithStringKey
{
    Task Encrypt(string value);

    Task<string> Decrypt();
}