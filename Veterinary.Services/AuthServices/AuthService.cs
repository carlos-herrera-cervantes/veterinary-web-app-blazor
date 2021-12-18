using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using Veterinary.Domain.Models;

namespace Veterinary.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        #region snippet_Properties

        private readonly HttpClient _httpClient;

        private readonly AuthenticationStateProvider _authenticationStateProvider;
        
        private readonly ILocalStorageService _localStorage;

        #endregion

        #region snippet_Constructors

        public AuthService
        (
            IHttpClientFactory clientFactory,
            AuthenticationStateProvider authenticationStateProvider,
            ILocalStorageService localStorage
        )
        {
            _httpClient = clientFactory.CreateClient("veterinary");
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        #endregion

        #region snippet_ActionMethods

        /// <summary>
        /// Authenticates an employee
        /// </summary>
        /// <param name="credentials">Employee number and password</param>
        /// <returns>AuthResponseDto</returns>
        public async Task<AuthResponseDto> SignInAsync(Credentials credentials)
        {
            var credentialsJson = new StringContent
            (
                JsonConvert.SerializeObject(credentials), Encoding.UTF8, "application/json"
            );

            using var httpResponse = await _httpClient.PostAsync("auth/sign-in", credentialsJson);
            var content = await httpResponse.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(content);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return authResponse;
            }

            await _localStorage.SetItemAsync("jwt", authResponse.Data);

            ((JwtAuthenticationStateProvider)_authenticationStateProvider)
                .MarkUserAsAuthenticated(credentials.EmployeeNumber);

            return authResponse;
        }

        /// <summary>
        /// Destroy an employee session
        /// </summary>
        public async Task SignOutAsync()
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            await _localStorage.RemoveItemAsync("jwt");
            ((JwtAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            using var httpResponse = await _httpClient.PostAsync("auth/sign-out", null);
        }

        #endregion
    }
}
