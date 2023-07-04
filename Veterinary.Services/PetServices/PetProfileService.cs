using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Veterinary.Domain.Types;
using Veterinary.Domain.Models;
using Veterinary.Domain.Config;

namespace Veterinary.Services.PetServices;

public class PetProfileService : IPetProfileService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorageService;

    private readonly ILogger _logger;

    #endregion

    #region snippet_Constructors

    public PetProfileService(
        IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorageService,
        ILogger<PetProfileService> logger
    )
    {
        _httpClient = httpClientFactory.CreateClient("veterinary");
        _localStorageService = localStorageService;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<HttpListResponse<PetProfile>> GetByCustomerIdAsync(string customerId)
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryPetPathV1}/profiles/{customerId}/customer");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible list pet profiles for customer: {customerId}. Status code: {httpResponse.StatusCode}");
            return new HttpListResponse<PetProfile>
            {
                Data = new List<PetProfile>()
            };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<HttpListResponse<PetProfile>>(content);
    }

    [ExcludeFromCodeCoverage]
    public async Task<HttpListResponse<PetProfile>> GetAllAsync()
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryPetPathV1}/profiles");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible list pet profiles. Status code: {httpResponse.StatusCode}");
            return new HttpListResponse<PetProfile>
            {
                Data = new List<PetProfile>()
            };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<HttpListResponse<PetProfile>>(content);
    }

    public async Task<PetProfile> GetByIdAsync(string id)
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryPetPathV1}/profiles/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible get pet profile for pet: {id}. Status code: {httpResponse.StatusCode}");
            return new PetProfile();
        }

        var content = await httpResponse.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<PetProfile>(content);
    }

    #endregion
}
