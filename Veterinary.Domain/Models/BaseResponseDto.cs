using Newtonsoft.Json;

namespace Veterinary.Domain.Models
{
    public class BaseResponseDto
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
    }

    public class AuthResponseDto : BaseResponseDto
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
