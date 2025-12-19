using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Jarvis.Web.Models;
using System.Net.Http.Json;

namespace Jarvis.Web.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider, IAuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private readonly AuthenticationState _anonymous;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http, IConfiguration config)
    {
        _localStorage = localStorage;
        _http = http;
        _http.BaseAddress = new Uri(config["IdentityApiUrl"]!);
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }
        catch (InvalidOperationException)
        {
            return _anonymous;
        }
    }

    public async Task<AuthResponseModel> Login(LoginModel loginModel)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", loginModel);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

        if (result!.Success)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(result.Token), "jwt"));
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(authenticatedUser)));
        }

        return result;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    public async Task<AuthResponseModel> Register(RegisterModel registerModel)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", registerModel);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseModel>();

        if (result!.Success)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(result.Token), "jwt"));
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(authenticatedUser)));
        }

        return result;
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var claims = new List<Claim>();

        foreach (var kvp in keyValuePairs!)
        {
            if (kvp.Key == "role" || kvp.Key == ClaimTypes.Role)
            {
                claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()!));
            }
            else
            {
                claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
            }
        }

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
}
