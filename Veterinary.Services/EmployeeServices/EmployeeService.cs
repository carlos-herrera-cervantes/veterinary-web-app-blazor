using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Veterinary.Domain.Models;

namespace Veterinary.Services.EmployeeServices
{
    public class EmployeeService : IEmployeeService
    {
        #region snippet_Properties

        private readonly HttpClient _httpClient;

        private readonly ILocalStorageService _localStorage;

        #endregion

        #region snippet_Constructors

        public EmployeeService(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
        {
            _httpClient = clientFactory.CreateClient("veterinary");
            _localStorage = localStorage;
        }

        #endregion

        #region snippet_ActionMethods

        /// <summary>Returns an enumerable of employees</summary>
        /// <returns>Enumerable of employees</returns>
        public async Task<ListEmployeeResponseDto> GetAllAsync()
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.GetAsync("employees");

            var content = await httpResponse.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<ListEmployeeResponseDto>(content);
        }

        /// <summary>Returns a employee by its ID</summary>
        /// <param name="id">Employee ID</param>
        /// <returns>SingleEmployeeResponseDto</returns>
        public async Task<SingleEmployeeResponseDto> GetByIdAsync(string id)
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.GetAsync($"employees/{id}");

            var content = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SingleEmployeeResponseDto>(content);
        }

        /// <summary>Register a new employee</summary>
        /// <param name="employee">Employee object</param>
        /// <returns>SingleEmployeeResponseDto</returns>
        public async Task<SingleEmployeeResponseDto> CreateAsync(Employee employee)
        {
            var employeeJson = new StringContent
            (
                JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"
            );

            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.PostAsync("employees", employeeJson);

            var content = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SingleEmployeeResponseDto>(content);
        }

        /// <summary>Updates partially an employee</summary>
        /// <param name="id">Employee ID</param>
        /// <param name="employee">Employee object</param>
        /// <returns>SingleEmployeeResponseDto</returns>
        public async Task<SingleEmployeeResponseDto> UpdateByIdAsync(string id, Employee employee)
        {
            var employeeJson = new StringContent
            (
                JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"
            );

            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.PatchAsync($"employees/{id}", employeeJson);

            var content = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SingleEmployeeResponseDto>(content);
        }

        /// <summary>Deletes an employee</summary>
        /// <param name="id">Employee ID</param>
        public async Task DeleteByIdAsync(string id)
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.DeleteAsync($"employees/{id}");
        }

        #endregion
    }
}