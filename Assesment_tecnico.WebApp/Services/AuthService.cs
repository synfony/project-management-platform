using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Assesment_tecnico.Shared.Models;
using Assesment_tecnico.WebApp.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Assesment_tecnico.WebApp.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        public async Task<string?> Login(LoginModel loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                var token = result.GetProperty("token").GetString();
                
                await _localStorage.SetItemAsync("authToken", token);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return null;
            }

            return "Login failed. Please check your credentials.";
        }

        public async Task<string?> Register(RegisterModel registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                if (result.TryGetProperty("token", out var tokenProperty) && tokenProperty.GetString() is string token)
                {
                    await _localStorage.SetItemAsync("authToken", token);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return null; // Success
                }
                return "Registration succeeded but no token was returned.";
            }

            // --- Robust Error Handling ---
            var errorContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(errorContent))
            {
                return $"An error occurred ({(int)response.StatusCode} {response.ReasonPhrase}).";
            }

            // Try to parse as ValidationProblemDetails (default for [ApiController] validation)
            try
            {
                var validationProblem = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (validationProblem?.Errors != null)
                {
                    var errorMessages = validationProblem.Errors.SelectMany(e => e.Value);
                    return string.Join("\n", errorMessages);
                }
            }
            catch (JsonException) { /* Not a ValidationProblemDetails, fall through */ }

            // Try to parse as IdentityError[] (for manual BadRequest(result.Errors))
            try
            {
                var identityErrors = JsonSerializer.Deserialize<IdentityError[]>(errorContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (identityErrors != null && identityErrors.Any())
                {
                    return string.Join("\n", identityErrors.Select(e => e.Description));
                }
            }
            catch (JsonException) { /* Not an IdentityError array, fall through */ }

            // If all else fails, return the raw content
            return errorContent;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)) ?? new List<Claim>();
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
    }
}
