using System.Net.Http.Json;
using Jarvis.Web.Models;
using Blazored.LocalStorage;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json;

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
        try
        {
            return await _httpClient.GetFromJsonAsync<VenueDetailDto>($"api/Venues/{venueId}");
        }
        catch
        {
            return null;
        }
    }

    public async Task CreateVenueAsync(CreateVenueModel model)
    {
        await AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Venues", model);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Create Failed ({response.StatusCode}): {errorContent}");
        }
    }

    public async Task<string> UploadReviewFormAsync(IBrowserFile file)
    {
        await AddAuthHeader();

        using var content = new MultipartFormDataContent();

        // Maksimum 2MB dosya boyutu limiti
        var maxFileSize = 2 * 1024 * 1024;

        var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));

        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        content.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync("api/Venues/upload-form", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {errorMsg}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (result.TryGetProperty("url", out var urlProperty))
        {
            return urlProperty.GetString() ?? "";
        }

        if (result.TryGetProperty("Url", out var urlPropCase))
        {
            return urlPropCase.GetString() ?? "";
        }

        return "";
    }

    public async Task<List<VenueDto>> GetMyManagedVenuesAsync()
    {
        await AddAuthHeader();
        var response = await _httpClient.GetFromJsonAsync<List<VenueDto>>("api/Venues/managed");
        return response ?? new List<VenueDto>();
    }
}