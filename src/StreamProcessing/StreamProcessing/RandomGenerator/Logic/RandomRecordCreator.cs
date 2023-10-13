using Bogus;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator.Logic;

internal sealed class RandomRecordCreator : IRandomRecordCreator
{
    private readonly Faker _faker;

    public RandomRecordCreator()
    {
        _faker = new Faker();
    }
    
    public PluginRecord Create(Dictionary<string, RandomType> columnTypesByName)
    {
        var record = new Dictionary<string, object>();

        foreach (var columnTypeByName in columnTypesByName)
        {
            record[columnTypeByName.Key] = GetRandom(columnTypeByName.Value);
        }

        return new PluginRecord { Record = record };
    }

    //TODO Boxing performance issue
    private object GetRandom(RandomType type)
    {
        return type switch
        {
            RandomType.Bool => _faker.Random.Bool(),
            RandomType.Guid => _faker.Random.Guid(),
            RandomType.Number => _faker.Random.Number(int.MinValue, int.MaxValue),
            RandomType.Text => _faker.Random.String(2, 100),
            RandomType.TimeSpan => _faker.Date.Timespan(),
            RandomType.DateTime => _faker.Date.Past(),
            RandomType.Age => _faker.Random.Number(1, 100),
            RandomType.Name => _faker.Name.FirstName(),
            RandomType.LastName => _faker.Name.LastName(),
            RandomType.Company => _faker.Company.CompanyName(),
            RandomType.Address => _faker.Address.FullAddress(),
            RandomType.PhoneNumber => _faker.Phone.PhoneNumber(),
            RandomType.Email => _faker.Internet.Email(),
            RandomType.Gender => _faker.Name.Prefix(),
            RandomType.Ip => _faker.Internet.Ip(),
            RandomType.Account => _faker.Finance.Account(),
            RandomType.MusicGenre => _faker.Music.Genre(),
            RandomType.CreditCardNumber => _faker.Finance.CreditCardNumber(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}