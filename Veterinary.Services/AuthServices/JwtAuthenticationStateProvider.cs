using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Veterinary.Services.AuthServices
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
            => _localStorage = localStorage;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var jwt = await _localStorage.GetItemAsync<string>("jwt");

            if (string.IsNullOrEmpty(jwt))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            return new AuthenticationState(new ClaimsPrincipal
            (
                new ClaimsIdentity(ParseClaimsFromJwt(jwt), "jwt")
            ));
        }

        #region snippet_Helpers

        /// <summary>
        /// Mark a user as authenticated user
        /// </summary>
        /// <param name="employeeNumber">Employee number</param>
        public void MarkUserAsAuthenticated(string employeeNumber)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            { 
                new Claim(ClaimTypes.Name, employeeNumber)
            }, "apiauth"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Mark a user as unauthenticated
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Extract the values of claims from Json Web Token
        /// </summary>
        /// <param name="jwtToken">Json Web Token</param>
        /// <returns>IEnumerable of claims</returns>
        private IEnumerable<Claim> ParseClaimsFromJwt(string jwtToken)
        {
            var claims = new List<Claim>();
            var payload = jwtToken.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        /// <summary>
        /// Decompose the base 64 string
        /// </summary>
        /// <param name="base64">Base 64 string</param>
        /// <returns>Array of bytes</returns>
        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        #endregion
    }
}