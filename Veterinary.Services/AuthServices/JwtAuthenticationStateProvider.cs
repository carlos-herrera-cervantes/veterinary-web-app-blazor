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
        #region snippet_Properties

        private readonly ILocalStorageService _localStorage;

        #endregion

        #region snippet_Constructors

        public JwtAuthenticationStateProvider(ILocalStorageService localStorage)
            => _localStorage = localStorage;

        #endregion

        #region snippet_ActionMethods

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

        #endregion

        #region snippet_Helpers

        public void MarkUserAsAuthenticated(string employeeNumber)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            { 
                new Claim(ClaimTypes.Name, employeeNumber)
            }, "apiauth"));

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

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
