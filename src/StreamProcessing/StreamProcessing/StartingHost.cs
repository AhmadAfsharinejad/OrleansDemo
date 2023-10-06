using Microsoft.Extensions.Hosting;
using Orleans.Concurrency;
using StreamProcessing.DummyOutput.Domain;
using StreamProcessing.Filter.Domain;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.TestGrains.Interfaces;

namespace StreamProcessing;

internal sealed class StartingHost : BackgroundService
{
    private readonly IGrainFactory _grainFactory;
    private readonly IScenarioRunner _scenarioRunner;

    public StartingHost(IGrainFactory grainFactory, IScenarioRunner scenarioRunner)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
        _scenarioRunner = scenarioRunner ?? throw new ArgumentNullException(nameof(scenarioRunner));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Start {DateTime.Now}");

        //await RandomWithGrain();
        //await Random();
        await RunScenario();

        Console.WriteLine($"Finished {DateTime.Now}");
    }

    private async Task RandomWithGrain()
    {
        var generator = _grainFactory.GetGrain<IIntRandomGeneratorGrain>(0);
        await generator.Compute();
    }

    private async Task Random()
    {
        var t1 = RunPassAwayGrain(1);
        //var t2 = RunPassAwayGrain(2);
        //var t3 = RunPassAwayGrain(3);

        await Task.WhenAll(t1); //, t2, t3);
    }

    private async Task RunPassAwayGrain(int grainId)
    {
        var grain = _grainFactory.GetGrain<IPassAwayGrain>(grainId);

        Task.Run(async () =>
        {
            if (grainId != 1) return;

            await Task.Delay(TimeSpan.FromSeconds(1));

            var passAwayGrain = _grainFactory.GetGrain<IPassAwayGrain>(grainId);
            await passAwayGrain.SayHello();
        });

        var batchCount = 10;
        var items = new int[batchCount];
        int index = 0;

        for (int i = 1; i < 10000000; i++)
        {
            items[index++] = i;

            if (index == batchCount)
            {
                try
                {
                    await grain.Compute(items.AsImmutable());
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }

                items = new int[batchCount];
                index = 0;
            }

            // if (i % 1000 == 0)
            // {
            //     await Task.WhenAll(tasks);
            //     tasks.Clear();
            // }
        }
    }

    private async Task RunScenario()
    {
        var config = GetScenarioConfig();
        await _scenarioRunner.Run(config);
    }

    private static ScenarioConfig GetScenarioConfig()
    {
        var configs = new List<PluginConfig>();
        var relations = new List<LinkConfig>();

        var randomPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.Random), Guid.NewGuid(), GetRandomGeneratorConfig());
        configs.Add(randomPluginConfig);

        var randomPluginConfig2 = new PluginConfig(new PluginTypeId(PluginTypeNames.Random), Guid.NewGuid(), GetRandomGeneratorConfig());
        configs.Add(randomPluginConfig2);
        
        var randomPluginConfig3 = new PluginConfig(new PluginTypeId(PluginTypeNames.Random), Guid.NewGuid(), GetRandomGeneratorConfig());
        configs.Add(randomPluginConfig3);
        
        var randomPluginConfig4 = new PluginConfig(new PluginTypeId(PluginTypeNames.Random), Guid.NewGuid(), GetRandomGeneratorConfig());
        configs.Add(randomPluginConfig4);

        var filterPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.Filter), Guid.NewGuid(), GetFilterConfig());
        configs.Add(filterPluginConfig);
        
        var dummyOutputPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.DummyOutput), Guid.NewGuid(), GetDummyOutputConfig());
        configs.Add(dummyOutputPluginConfig);

        relations.Add(new LinkConfig(randomPluginConfig.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(randomPluginConfig2.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(randomPluginConfig3.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(randomPluginConfig4.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(filterPluginConfig.Id, dummyOutputPluginConfig.Id));

        return new ScenarioConfig
        {
            Id = Guid.NewGuid(),
            Configs = configs,
            Relations = relations
        };
    }

    private static RandomGeneratorConfig GetRandomGeneratorConfig()
    {
        return new RandomGeneratorConfig
        {
            Columns = new List<RandomColumn>
            {
                new("Name", RandomType.Name, FieldType.Text),
                new("Age", RandomType.Age, FieldType.Integer),
                new("LastName", RandomType.LastName, FieldType.Text),
                new("DateTime", RandomType.DateTime, FieldType.DateTime),
            },
            Count = 2000000,
            BatchCount = 10
        };
    }

    private static FilterConfig GetFilterConfig()
    {
        var constraints = new List<IConstraint>();
        constraints.Add(new FieldConstraint
        {
            Operator = ConstraintOperators.Greater,
            FieldName = "Age",
            Value = 15
        });
        constraints.Add(new FieldConstraint
        {
            Operator = ConstraintOperators.Less,
            FieldName = "Age",
            Value = 40
        });

        return new FilterConfig
        {
            Constraint = new LogicalConstraint { Operator = ConstraintOperator.And , Constraints = constraints}
        };
    }

    private static DummyOutputConfig GetDummyOutputConfig()
    {
        return new DummyOutputConfig
        {
            IsWriteEnabled = true,
            RecordCountInterval = 100000,
        };
    }
}