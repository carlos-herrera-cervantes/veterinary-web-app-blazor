using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Veterinary.Domain.Models
{
    public class Employee
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }

        [JsonProperty("municipality")]
        public string Municipality { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("colony")]
        public string Colony { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }

    public class SingleEmployeeDto : Employee
    {
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class SingleEmployeeResponseDto : BaseResponseDto
    {
        public SingleEmployeeDto Data { get; set; }
    }

    public class ListEmployeeResponseDto : BaseResponseDto
    {
        public IEnumerable<SingleEmployeeDto> Data { get; set; }
    }
}