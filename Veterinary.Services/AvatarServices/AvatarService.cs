using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices;

public class AvatarService : IAvatarService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly ILocalStorageService _localStorage;

    #endregion

    #region snippet_Constructors

    public AvatarService(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _localStorage = localStorage;
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
            return null;
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
            
        return JsonConvert.DeserializeObject<Avatar>(content);
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
