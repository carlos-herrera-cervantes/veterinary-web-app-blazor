using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Veterinary.Domain.Config;
using Veterinary.Domain.Models;

namespace Veterinary.Services.CustomerServices;

public class CustomerAvatarService : ICustomerAvatarService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorageService;

    private readonly ILogger<CustomerAvatarService> _logger;

    #endregion

    #region snippet_Constructors

    public CustomerAvatarService(
        IHttpClientFactory clientFactory,
        ILocalStorageService localStorageService,
        ILogger<CustomerAvatarService> logger
    )
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _localStorageService = localStorageService;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<CustomerAvatar> GetByIdAsync(string id)
    {
        var jwt = await _localStorageService.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"{ApiConfig.VeterinaryCustomerPathV1}/avatar/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible get avatar for user: {id}");
            return new CustomerAvatar { Path = AvatarConfig.NoProfilePicture };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
        var avatar = JsonConvert.DeserializeObject<CustomerAvatar>(content);

        try
        {
            using var avatarStatusResponse = await _httpClient.GetAsync(avatar.Path);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Avatar not found for user: {id}");
                avatar.Path = AvatarConfig.NoProfilePicture;
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception checking avatar for user: {id}");
            _logger.LogWarning(e.Message);
            avatar.Path = AvatarConfig.NoProfilePicture;
        }

        return avatar;
    }

    #endregion
}
