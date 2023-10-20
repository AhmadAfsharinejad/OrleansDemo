using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using StreamProcessing.DummyOutput.Domain;
using StreamProcessing.Filter.Domain;
using StreamProcessing.Filter.Interfaces;
using StreamProcessing.HttpListener.Domain;
using StreamProcessing.HttpResponse.Domain;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.RandomGenerator.Domain;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Scenario.Domain;
using StreamProcessing.Scenario.Interfaces;
using StreamProcessing.SqlExecutor.Domain;

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
        var sw = new Stopwatch();
        sw.Start();
        Console.WriteLine($"Start {DateTime.Now}");

        //await RunScenario();
        //await RunScenario2();
        await RunScenario3();

        sw.Stop();
        Console.WriteLine($"Finished {DateTime.Now} {sw.Elapsed.TotalMilliseconds}");
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

        var filterPluginConfig2 = new PluginConfig(new PluginTypeId(PluginTypeNames.Filter), Guid.NewGuid(), GetFilterConfig());
        configs.Add(filterPluginConfig2);

        var sqlExecutorConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.SqlExecutor), Guid.NewGuid(), GetSqlExecutorConfig());
        configs.Add(sqlExecutorConfig);

        var dummyOutputPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.DummyOutput), Guid.NewGuid(), GetDummyOutputConfig());
        configs.Add(dummyOutputPluginConfig);

        relations.Add(new LinkConfig(randomPluginConfig.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(randomPluginConfig2.Id, filterPluginConfig.Id));
        relations.Add(new LinkConfig(randomPluginConfig3.Id, filterPluginConfig2.Id));
        relations.Add(new LinkConfig(randomPluginConfig4.Id, filterPluginConfig2.Id));
        relations.Add(new LinkConfig(filterPluginConfig.Id, dummyOutputPluginConfig.Id));
        relations.Add(new LinkConfig(filterPluginConfig2.Id, sqlExecutorConfig.Id));
        relations.Add(new LinkConfig(sqlExecutorConfig.Id, dummyOutputPluginConfig.Id));

        return new ScenarioConfig
        {
            Id = Guid.NewGuid(),
            Configs = configs,
            Relations = relations
        };
    }

    private async Task RunScenario2()
    {
        var config = GetScenarioConfig2();
        await _scenarioRunner.Run(config);
    }

    private static ScenarioConfig GetScenarioConfig2()
    {
        var configs = new List<PluginConfig>();
        var relations = new List<LinkConfig>();

        var httpPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.HttpListener), Guid.NewGuid(), GetHttpListenerConfig());
        configs.Add(httpPluginConfig);

        var HttpResponseConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.HttpResponse), Guid.NewGuid(), GetHttpResponseConfig());
        configs.Add(HttpResponseConfig);

        relations.Add(new LinkConfig(httpPluginConfig.Id, HttpResponseConfig.Id));

        return new ScenarioConfig
        {
            Id = Guid.NewGuid(),
            Configs = configs,
            Relations = relations
        };
    }
    
    private async Task RunScenario3()
    {
        var config = GetScenarioConfig3();
        await _scenarioRunner.Run(config);
    }

    private static ScenarioConfig GetScenarioConfig3()
    {
        var configs = new List<PluginConfig>();
        var relations = new List<LinkConfig>();

        var randomPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.Random), Guid.NewGuid(), GetRandomGeneratorConfig2());
        configs.Add(randomPluginConfig);

        var restConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.Rest), Guid.NewGuid(), GetRestConfig());
        configs.Add(restConfig);
        
        var dummyOutputPluginConfig = new PluginConfig(new PluginTypeId(PluginTypeNames.DummyOutput), Guid.NewGuid(), GetDummyOutputConfig());
        configs.Add(dummyOutputPluginConfig);

        relations.Add(new LinkConfig(randomPluginConfig.Id, restConfig.Id));
        relations.Add(new LinkConfig(restConfig.Id, dummyOutputPluginConfig.Id));

        return new ScenarioConfig
        {
            Id = Guid.NewGuid(),
            Configs = configs,
            Relations = relations
        };
    }

    private static RestConfig GetRestConfig()
    {
        return new RestConfig
        {
            JoinType = RecordJoinType.Append,
            Uri = "http://localhost:5089/Math/Add",
            HttpMethod = HttpMethod.Get,
            QueryStrings = new[] { new QueryStringField("b", "num") },
            StaticQueryStrings = new[] { new KeyValuePair<string, string>("a", "1") },
            RequestStaticHeaders = new[] { new KeyValuePair<string, string>("Accept", "application/json") },
            ResponseContentFieldName = "response",
            StatusFieldName = "status",
        };
    }

    private static HttpResponseConfig GetHttpResponseConfig()
    {
        return new HttpResponseConfig
        {
            Content = "By",
            StaticHeaders = new[] { new KeyValuePair<string, string>("id1", "1") },
            Headers = new[] { new HeaderField("resId", "fieldId") }
        };
    }

    private static HttpListenerConfig GetHttpListenerConfig()
    {
        return new HttpListenerConfig
        {
            Uri = "http://localhost:1380/index/",
            Headers = new[] { new HeaderField("id", "fieldId") }
        };
    }

    private static RandomGeneratorConfig GetRandomGeneratorConfig()
    {
        return new RandomGeneratorConfig
        {
            Columns = new List<RandomColumn>
            {
                new(new("Name", FieldType.Text), RandomType.Name),
                new(new("Age", FieldType.Integer), RandomType.Age),
                new(new("LastName", FieldType.Text), RandomType.LastName),
                new(new("DateTime", FieldType.DateTime), RandomType.DateTime),
            },
            Count = 1000000,
            BatchCount = 10
        };
    }
    
    private static RandomGeneratorConfig GetRandomGeneratorConfig2()
    {
        return new RandomGeneratorConfig
        {
            Columns = new List<RandomColumn>
            {
                new(new("num", FieldType.Integer), RandomType.Number)
            },
            Count = 100000,
            BatchCount = 10
        };
    }

    private static FilterConfig GetFilterConfig()
    {
        var constraints = new List<IConstraint>
        {
            new FieldConstraint
            {
                Operator = ConstraintOperators.Greater,
                FieldName = "Age",
                Value = 15
            },
            new FieldConstraint
            {
                Operator = ConstraintOperators.Less,
                FieldName = "Age",
                Value = 40
            }
        };

        return new FilterConfig
        {
            Constraint = new LogicalConstraint { Operator = ConstraintOperator.And, Constraints = constraints }
        };
    }

    private static SqlExecutorConfig GetSqlExecutorConfig()
    {
        return new SqlExecutorConfig
        {
            ConnectionString = @"Driver={ClickHouse ODBC Driver (Unicode)};Host=localhost;PORT=8123;Timeout=500;Username=admin;Password=admin",
            JoinType = RecordJoinType.Append,
            DqlCommand = new DqlCommand
            {
                CommandText = @"SELECT now() as dateTime, ? as age",
                ParameterFields = new[] { "Age" },
                OutputFields = new[]
                {
                    new DqlField("dateTime", new StreamField("db_dateTime", FieldType.DateTime)),
                    new DqlField("age", new StreamField("db_age", FieldType.Integer))
                }
            },
            DmlCommands = null
        };
    }

    private static DummyOutputConfig GetDummyOutputConfig()
    {
        return new DummyOutputConfig
        {
            IsWriteEnabled = true,
            RecordCountInterval = 10000,
        };
    }
}