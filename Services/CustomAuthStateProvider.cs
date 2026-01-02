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
        // Eğer config'den okuyamazsa varsayılan adresi kullan (Hata önleyici)
        var apiUrl = config["IdentityApiUrl"] ?? "https://localhost:7041";
        _http.BaseAddress = new Uri(apiUrl);
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // LocalStorage okuma işlemi bazen prerendering sırasında hata verebilir, try-catch şart.
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            // Token formatı bozuksa Parse işlemi patlayabilir
            var claims = ParseClaimsFromJwt(token);

            if (claims == null || !claims.Any())
                return _anonymous;

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }
        catch (Exception ex)
        {
            // Hata oluşursa konsola yaz ama uygulamayı ÇÖKERTME, anonim döndür.
            Console.WriteLine($"Auth Error: {ex.Message}");
            return _anonymous;
        }
    }

    public async Task<AuthResponseModel> Login(LoginModel loginModel)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
            {
                return new AuthResponseModel { Success = false, Message = "Sunucu hatası veya geçersiz bilgiler." };
            }

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
        catch (Exception ex)
        {
            return new AuthResponseModel { Success = false, Message = ex.Message };
        }
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    public async Task<AuthResponseModel> Register(RegisterModel registerModel)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", registerModel);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return new AuthResponseModel { Success = false, Message = $"Kayıt başarısız: {content}" };
            }

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
        catch (Exception ex)
        {
            return new AuthResponseModel { Success = false, Message = ex.Message };
        }
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            if (string.IsNullOrEmpty(jwt) || !jwt.Contains("."))
                return new List<Claim>();

            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            var claims = new List<Claim>();

            if (keyValuePairs != null)
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (kvp.Key == "role" || kvp.Key == ClaimTypes.Role)
                    {
                        // Rol array olarak gelirse (birden fazla rol) veya string gelirse
                        if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in element.EnumerateArray())
                            {
                                claims.Add(new Claim(ClaimTypes.Role, item.ToString()));
                            }
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()!));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, kvp.Value.ToString()!));
                    }
                }
            }

            return claims;
        }
        catch
        {
            // Token parse edilemezse boş liste dön
            return new List<Claim>();
        }
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