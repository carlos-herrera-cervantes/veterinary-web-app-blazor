using Newtonsoft.Json;

namespace Veterinary.Domain.Models;

public class Avatar
{
    [JsonProperty("employee_id")]
    public string EmployeeId { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }
}

public class CustomerAvatar
{
    [JsonProperty("customer_id")]
    public string CustomerId { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }
}
