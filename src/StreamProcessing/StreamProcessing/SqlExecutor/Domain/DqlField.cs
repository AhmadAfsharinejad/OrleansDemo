using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.SqlExecutor.Domain;

public record struct DqlField(string DbName, StreamField Field);