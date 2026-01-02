using System.Net.Http.Json;
using Jarvis.Web.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace Jarvis.Web.Services;

public class VenueApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public VenueApiService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task AddAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<VenueDto>> GetAllVenuesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VenueDto>>("api/Venues") ?? new();
    }

    public async Task<VenueDetailDto?> GetVenueDetailsAsync(Guid venueId)
    {
        try { return await _httpClient.GetFromJsonAsync<VenueDetailDto>($"api/Venues/{venueId}"); }
        catch { return null; }
    }

    public async Task CreateVenueAsync(CreateVenueModel model)
    {
        await AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Venues", model);

        // GÜNCELLEME: Hata varsa içeriğini oku ve fırlat
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create Failed ({response.StatusCode}): {errorContent}");
        }
    }
}