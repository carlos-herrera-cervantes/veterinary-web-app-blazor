using System.Collections.Generic;
using Newtonsoft.Json;

namespace Veterinary.Domain.Types;

public class HttpMessageResponse
{
    [JsonProperty("message")]
    public string Message { get; set; }
}

public class HttpListResponse<T> where T : class
{
    [JsonProperty("next")]
    public int Next { get; set; }

    [JsonProperty("previous")]
    public int Previous { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("data")]
    public IEnumerable<T> Data { get; set; }
}
