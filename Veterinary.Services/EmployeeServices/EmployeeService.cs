using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;
using Veterinary.Domain.Config;

namespace Veterinary.Services.EmployeeServices;

public class EmployeeService : IEmployeeService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorage;

    private readonly ILogger _logger;

    #endregion

    #region snippet_Constructors

    public EmployeeService(
        IHttpClientFactory clientFactory,
        ILocalStorageService localStorage,
        ILogger<EmployeeService> logger
    )
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _localStorage = localStorage;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<HttpListResponse<EmployeeProfile>> GetAllAsync(int offset, int limit)
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryEmployeePathV1}/profiles?offset={offset}&limit={limit}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Impossible list employees. Status code: {httpResponse.StatusCode}");
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<HttpListResponse<EmployeeProfile>>(content);
    }

    public async Task<EmployeeProfile> GetAsync()
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryEmployeePathV1}/profiles/me");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible get employee detail. Status code: {httpResponse.StatusCode}");
            return new EmployeeProfile();
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<EmployeeProfile>(content);
    }

    public async Task<EmployeeProfile> GetByIdAsync(string id)
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryEmployeePathV1}/profiles/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Impossible get employee detail. Status code: {httpResponse.StatusCode}");
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<EmployeeProfile>(content);
    }

    public async Task<EmployeeProfile> UpdateByIdAsync(string id, UpdateEmployeeProfileDto employee)
    {
        var employeeJson = new StringContent
        (
            JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json"
        );

        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.PatchAsync($"{ApiConfig.VeterinaryEmployeePathV1}/profiles/{id}", employeeJson);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Impossible update employee. Status code: {httpResponse.StatusCode}");
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<EmployeeProfile>(content);
    }

    #endregion
}
