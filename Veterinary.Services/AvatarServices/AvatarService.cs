using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Veterinary.Domain.Models;
using Veterinary.Domain.Config;

namespace Veterinary.Services.AuthServices;

public class AvatarService : IAvatarService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorage;

    private readonly ILogger<AvatarService> _logger;

    #endregion

    #region snippet_Constructors

    public AvatarService(
        IHttpClientFactory clientFactory,
        ILocalStorageService localStorage,
        ILogger<AvatarService> logger
    )
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _localStorage = localStorage;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<Avatar> GetAsync()
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"employees/avatar/me");

        if (!httpResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Imposible get avatar for current user with jwt: {jwt}");
            return new Avatar { Path = AvatarConfig.NoProfilePicture };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
        var avatar = JsonConvert.DeserializeObject<Avatar>(content);

        try
        {
            using var avatarStatusResponse = await _httpClient.GetAsync(avatar.Path);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Avatar not found for current user with jwt: {jwt}");
                avatar.Path = AvatarConfig.NoProfilePicture;
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception checking avatar for current user with jwt: {jwt}");
            _logger.LogWarning(e.Message);
            avatar.Path = AvatarConfig.NoProfilePicture;
        }

        return avatar;
    }

    public async Task<Avatar> GetByIdAsync(string id)
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.GetAsync($"employees/avatar/{id}");

        if (!httpResponse.IsSuccessStatusCode)
        {
            return new Avatar{ Path = AvatarConfig.NoProfilePicture };
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
        var avatar = JsonConvert.DeserializeObject<Avatar>(content);

        try
        {
            using var avatarStatusResponse = await _httpClient.GetAsync(avatar.Path);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Avatar not found user: {id}");
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

    public async Task<Avatar> Upload(MultipartFormDataContent content)
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.PostAsync($"employees/avatar/me", content);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Imposible upload the avatar. Status code: {httpResponse.StatusCode}");
        }

        var avatarContent = await httpResponse.Content.ReadAsStringAsync();
        var avatar = JsonConvert.DeserializeObject<Avatar>(avatarContent);

        await _localStorage.SetItemAsync("avatar", avatar.Path);

        return avatar;
    }

    #endregion
}
