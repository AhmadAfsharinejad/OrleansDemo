using StreamProcessing.Rest.Domain;
using StreamProcessing.Rest.Interfaces;

namespace StreamProcessing.Rest.Logic;

internal sealed class RestResponseConverter : IRestResponseConverter
{
    public async Task<IReadOnlyDictionary<string, object>> Convert(HttpResponseMessage responseMessage, RestConfig config)
    {
        var record = new Dictionary<string, object>();

        if (config.ResponseHeaders is not null)
        {
            foreach (var header in config.ResponseHeaders)
            {
                record[header.FieldName] = string.Join(",", responseMessage.Headers.GetValues(header.NameInHeader));
            }
        }

        if (!string.IsNullOrWhiteSpace(config.StatusFieldName))
        {
            record[config.StatusFieldName] = (int)responseMessage.StatusCode;
        }
        
        if (!string.IsNullOrWhiteSpace(config.ResponseContentFieldName))
        {
            record[config.ResponseContentFieldName] = await responseMessage.Content.ReadAsStringAsync();
        }

        return record;
    }
}