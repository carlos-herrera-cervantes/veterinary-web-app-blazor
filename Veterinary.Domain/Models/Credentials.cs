using Newtonsoft.Json;

namespace Veterinary.Domain.Models
{
    public class Credentials
    {
        [JsonProperty("employee_number")]
        public string EmployeeNumber { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
