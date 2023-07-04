using System;
using Newtonsoft.Json;

namespace Veterinary.Domain.Models;

public class PetProfile
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("customer_id")]
    public string CustomerId { get; set; }

    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("birthday")]
    public DateTime Birthday { get; set; }

    [JsonProperty("race_id")]
    public string RaceId { get; set; }

    [JsonProperty("classification_id")]
    public string ClassificationId { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
