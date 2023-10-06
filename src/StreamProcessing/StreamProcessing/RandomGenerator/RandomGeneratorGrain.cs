using Bogus;
using Orleans.Concurrency;
using StreamProcessing.PluginCommon;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.PluginCommon.Interfaces;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.RandomGenerator.Interfaces;

namespace StreamProcessing.RandomGenerator;

[StatelessWorker]
internal sealed class RandomGeneratorGrain : PluginGrain<RandomGeneratorConfig>, IRandomGeneratorGrain
{
    public RandomGeneratorGrain(IPluginGrainFactory pluginGrainFactory) : base(pluginGrainFactory)
    {
    }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"RandomGeneratorGrain Activated  {this.GetGrainId()}");
        return base.OnActivateAsync(cancellationToken);
    }
    
    [ReadOnly]
    public async Task Compute(Guid scenarioId, Guid pluginId,
        Immutable<PluginRecords>? pluginRecords,
        GrainCancellationToken cancellationToken)
    {

        var faker = new Faker();
        faker.Random.Guid();

        var config = await GetConfig(scenarioId, pluginId);
        
        var records = new List<PluginRecord>(config.BatchCount);

        var columnTypeByName = config.Columns.ToDictionary(x => x.Name, y => y.Type);

        for (int i = 0; i < config.Count; i++)
        {
            records.Add(Create(faker, columnTypeByName));

            if (config.BatchCount == records.Count)
            {
                await CallOutputs(scenarioId, pluginId, records, cancellationToken);
                records = new List<PluginRecord>(config.BatchCount);
            }
        }

        await CallOutputs(scenarioId, pluginId, records, cancellationToken);
    }

    private static PluginRecord Create(Faker faker, Dictionary<string, RandomType> columnTypesByName)
    {
        var record = new Dictionary<string, object>();

        foreach (var columnTypeByName in columnTypesByName)
        {
            record[columnTypeByName.Key] = GetRandom(faker, columnTypeByName.Value);
        }

        return new PluginRecord { Record = record };
    }

    //TODO Boxing performance issue
    private static object GetRandom(Faker faker, RandomType type)
    {
        return type switch
        {
            RandomType.Bool => faker.Random.Bool(),
            RandomType.Guid => faker.Random.Guid(),
            RandomType.Number => faker.Random.Number(int.MinValue, int.MaxValue),
            RandomType.Text => faker.Random.String(2, 100),
            RandomType.TimeSpan => faker.Date.Timespan(),
            RandomType.DateTime => faker.Date.Past(),
            RandomType.Age => faker.Random.Number(1, 100),
            RandomType.Name => faker.Person.FirstName,
            RandomType.LastName => faker.Person.LastName,
            RandomType.Company => faker.Person.Company,
            RandomType.Address => faker.Person.Address,
            RandomType.Email => faker.Person.Email,
            RandomType.Gender => faker.Person.Gender,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}