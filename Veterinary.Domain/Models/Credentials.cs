using Newtonsoft.Json;

namespace Veterinary.Domain.Models
{
    public class Credentials
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
