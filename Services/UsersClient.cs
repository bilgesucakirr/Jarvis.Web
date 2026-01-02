using Blazored.LocalStorage;
using Jarvis.Web.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Jarvis.Web.Services;

public class UsersClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public UsersClient(HttpClient httpClient, ILocalStorageService localStorage)
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

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<UserProfileDto>("api/users/profile");
    }

    public async Task UpdateProfileAsync(UpdateProfileRequest request)
    {
        await AddAuthHeader();
        var response = await _httpClient.PutAsJsonAsync("api/users/profile", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<UserListDto>> GetAllUsersAsync()
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<UserListDto>>("api/users") ?? new();
    }

    public async Task AssignRoleAsync(string userId, string role)
    {
        await AddAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/users/assign-role", new AssignRoleRequest { UserId = userId, Role = role });
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Role assignment failed.");
        }
    }

    public async Task<List<ReviewerDto>> SearchReviewersAsync(string? keyword)
    {
        await AddAuthHeader();
        var url = string.IsNullOrEmpty(keyword) ? "api/users/reviewers" : $"api/users/reviewers?keyword={keyword}";
        return await _httpClient.GetFromJsonAsync<List<ReviewerDto>>(url) ?? new();
    }
}