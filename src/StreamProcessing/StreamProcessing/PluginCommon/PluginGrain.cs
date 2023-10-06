namespace StreamProcessing.PluginCommon;

internal abstract class PluginGrain<TConfig> : Grain
{
    private TConfig? _config;

    public Task NotifyConfigChange(Guid pluginId)
    {
        return Task.CompletedTask;
    }

    protected async Task<TConfig> GetConfig(Guid pluginId)
    {
        //TODO mishe ba in raft - aggregation moshkel nadare?
        //var pluginId = this.GetPrimaryKey();
        
        if (_config is null)
        {
            throw new NotImplementedException();
        }
        
        return _config;
    }
}