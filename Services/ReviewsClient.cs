using Blazored.LocalStorage;
using Jarvis.Web.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Jarvis.Web.Services;

public class ReviewsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public ReviewsClient(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    private async Task AddAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task SubmitReviewAsync(Guid assignmentId, SubmitReviewModel model, IBrowserFile? file)
    {
        await AddAuthHeader();

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.OverallScore.ToString()), "OverallScore");
        content.Add(new StringContent(model.Confidence.ToString()), "Confidence");
        content.Add(new StringContent(model.CommentsToAuthor), "CommentsToAuthor");

        if (model.CommentsToEditor is not null)
            content.Add(new StringContent(model.CommentsToEditor), "CommentsToEditor");

        var response = await _httpClient.PostAsync($"api/Reviews/{assignmentId}/submit", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ReviewAssignmentModel>> GetMyAssignmentsAsync()
    {
        await AddAuthHeader(); 

        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ReviewAssignmentModel>>("api/Reviews/my-assignments");
            return result ?? new List<ReviewAssignmentModel>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            
            return new List<ReviewAssignmentModel>();
        }
    }
    public async Task<ReviewAssignmentDetailDto?> GetAssignmentDetailAsync(Guid assignmentId)
    {
        await AddAuthHeader();
        try
        {
            return await _httpClient.GetFromJsonAsync<ReviewAssignmentDetailDto>($"api/Assignments/{assignmentId}");
        }
        catch
        {
            return null;
        }
    }
}
