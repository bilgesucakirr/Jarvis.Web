using System.Net.Http.Json;

namespace Jarvis.Web.Services;

public class SubmissionApiService
{
    private readonly HttpClient _httpClient;

    public SubmissionApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateSubmissionAsync(MultipartFormDataContent content)
    {
        var response = await _httpClient.PostAsync("api/Submissions", content);
        return response.IsSuccessStatusCode;
    }
}