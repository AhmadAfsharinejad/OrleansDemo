using System.Web;
using StreamProcessing.PluginCommon.Domain;
using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest.Logic;

internal sealed class UriReplacer : IUriReplacer
{
    public string GetUri(RestConfig config, PluginRecord pluginRecord)
    {
        var builder = new UriBuilder(config.Uri);
        var query = HttpUtility.ParseQueryString(builder.Query);

        if (config.QueryStrings is not null)
        {
            foreach (var queryString in config.QueryStrings)
            {
                query[queryString.NameInQueryString] = pluginRecord.Record[queryString.FieldName].ToString();
            }
        }

        if (config.StaticQueryStrings is not null)
        {
            foreach (var queryString in config.StaticQueryStrings)
            {
                query[queryString.Key] = queryString.Value;
            }
        }

        builder.Query = query.ToString();
        return builder.ToString();
    }
}