using Blazored.LocalStorage;
using Jarvis.Web.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<string> UploadProfilePictureAsync(IBrowserFile file)
    {
        await AddAuthHeader();

        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(2 * 1024 * 1024));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _httpClient.PostAsync("api/users/profile-picture", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Upload failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("url").GetString() ?? "";
    }

    public async Task<List<AreaOfInterestDto>> GetInterestsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<AreaOfInterestDto>>("api/interests") ?? new();
    }

    public async Task AddInterestAsync(string name)
    {
        await AddAuthHeader();
        await _httpClient.PostAsJsonAsync("api/interests", name);
    }

    public async Task DeleteInterestAsync(int id)
    {
        await AddAuthHeader();
        await _httpClient.DeleteAsync($"api/interests/{id}");
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

    public async Task<List<ReviewerDto>> SearchReviewersAsync(string? keyword, string? interest = null)
    {
        await AddAuthHeader();
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={keyword}");
        if (!string.IsNullOrEmpty(interest)) queryParams.Add($"interest={interest}");

        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";

        return await _httpClient.GetFromJsonAsync<List<ReviewerDto>>($"api/users/reviewers{queryString}") ?? new();
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        await AddAuthHeader();
        var payload = new { CurrentPassword = currentPassword, NewPassword = newPassword };
        var response = await _httpClient.PostAsJsonAsync("api/users/change-password", payload);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(); // JSON içindeki mesajı almak gerekebilir
            throw new Exception("Password change failed. Check your current password.");
        }
    }
}