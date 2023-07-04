using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Veterinary.Domain.Config;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;

namespace Veterinary.Services.CustomerServices;

public class CustomerProfileService : ICustomerProfileService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorageService;

    private readonly ILogger _logger;

    #endregion

    #region snippet_Constructors

    public CustomerProfileService(
        IHttpClientFactory clientFactory,
        ILocalStorageService localStorageService,
        ILogger<CustomerProfileService> logger
    )
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _localStorageService = localStorageService;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<HttpListResponse<CustomerProfile>> GetAllAsync(int offset, int limit)
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryCustomerPathV1}/profiles?offset={offset}&limit={limit}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible list customers. Status code: {httpResponse.StatusCode}");
            return new HttpListResponse<CustomerProfile>
            {
                Data = new List<CustomerProfile>()
            };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<HttpListResponse<CustomerProfile>>(content);
    }

    public async Task<CustomerProfile> GetByIdAsync(string id)
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryCustomerPathV1}/profiles/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible get customer detail: {id}. Status code: {httpResponse.StatusCode}");
            return new CustomerProfile();
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<CustomerProfile>(content);
    }

    #endregion
}
