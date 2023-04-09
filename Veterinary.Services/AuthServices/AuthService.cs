using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Veterinary.Domain.Models;
using Veterinary.Domain.Types;
using Veterinary.Domain.Config;

namespace Veterinary.Services.AuthServices;

public class AuthService : IAuthService
{
    #region snippet_Properties

    private readonly HttpClient _httpClient;

    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private readonly ILocalStorageService _localStorage;

    private readonly ILogger _logger;

    #endregion

    #region snippet_Constructors

    public AuthService(
        IHttpClientFactory clientFactory,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage,
        ILogger<AuthService> logger
    )
    {
        _httpClient = clientFactory.CreateClient("veterinary");
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
        _logger = logger;
    }

    #endregion

    #region snippet_ActionMethods

    public async Task<HttpMessageResponse> SignInAsync(Credentials credentials)
    {
        var credentialsJson = new StringContent
        (
            JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"
        );

        using var httpResponse = await _httpClient.PostAsync($"{ApiConfig.VeterinaryAuthorizerPathV1}/sign-in", credentialsJson);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Impossible to sign in. Status code: {httpResponse.StatusCode}");
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<HttpMessageResponse>(content);

        await _localStorage.SetItemAsync("jwt", authResponse.Message);

        ((JwtAuthenticationStateProvider)_authenticationStateProvider)
            .MarkUserAsAuthenticated(credentials.Email);

        return authResponse;
    }

    public async Task<HttpMessageResponse> SignupEmployeeAsync(Credentials credentials)
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");
        var credentialsJson = new StringContent
        (
            JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"
        );

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient
            .PostAsync($"{ApiConfig.VeterinaryAuthorizerPathV1}/sign-up/employees", credentialsJson);

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Impossible create new employee. Status code: {httpResponse.StatusCode}");
        }

        var content = await httpResponse.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<HttpMessageResponse>(content);

        return authResponse;
    }

    public async Task SignOutAsync()
    {
        var jwt = await _localStorage.GetItemAsync<string>("jwt");

        await _localStorage.RemoveItemAsync("jwt");
        await _localStorage.RemoveItemAsync("name");
        await _localStorage.RemoveItemAsync("avatar");

        ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        using var httpResponse = await _httpClient.PostAsync($"{ApiConfig.VeterinaryAuthorizerPathV1}/sign-out", null);
    }

    #endregion
}
