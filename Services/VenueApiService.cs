using System.Net.Http.Json;
using Jarvis.Web.Models;

namespace Jarvis.Web.Services;

public class VenueApiService
{
    private readonly HttpClient _httpClient;

    public VenueApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
}