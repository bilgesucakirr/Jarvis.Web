using Jarvis.Web.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using System.Text.Json;

namespace Jarvis.Web.Services;

public class SubmissionsClient
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public SubmissionsClient(HttpClient httpClient, ILocalStorageService _localStorage)
    {
        _httpClient = httpClient;
        this._localStorage = _localStorage;
    }

    private async Task AddAuthHeader()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<Guid> SubmitAsync(SubmissionModel model, IBrowserFile file)
    {
        await AddAuthHeader();

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(model.VenueId.ToString()), "VenueId");
        content.Add(new StringContent(model.VenueEditionId.ToString()), "VenueEditionId");
        content.Add(new StringContent(model.CallForPapersId.ToString()), "CallForPapersId");
        content.Add(new StringContent(model.TrackId.ToString()), "TrackId");
        content.Add(new StringContent(model.Title), "Title");
        content.Add(new StringContent(model.Abstract), "Abstract");
        content.Add(new StringContent(model.Keywords), "Keywords");
        content.Add(new StringContent(model.Type.ToString()), "Type");
        content.Add(new StringContent(model.IsOriginal.ToString()), "IsOriginal");
        content.Add(new StringContent(model.IsNotElsewhere.ToString()), "IsNotElsewhere");
        content.Add(new StringContent(model.HasConsent.ToString()), "HasConsent");
        content.Add(new StringContent(model.SubmitterEmail), "SubmitterEmail");
        content.Add(new StringContent(model.SubmitterName), "SubmitterName");

        for (int i = 0; i < model.Authors.Count; i++)
        {
            content.Add(new StringContent(model.Authors[i].FirstName), $"Authors[{i}].FirstName");
            content.Add(new StringContent(model.Authors[i].LastName), $"Authors[{i}].LastName");
            content.Add(new StringContent(model.Authors[i].Email), $"Authors[{i}].Email");
            content.Add(new StringContent(model.Authors[i].Affiliation), $"Authors[{i}].Affiliation");
            content.Add(new StringContent(model.Authors[i].Country), $"Authors[{i}].Country");
            content.Add(new StringContent(model.Authors[i].IsCorresponding.ToString()), $"Authors[{i}].IsCorresponding");
        }

        var fileContent = new StreamContent(file.OpenReadStream(1024 * 1024 * 10));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "ManuscriptFile", file.Name);

        var response = await _httpClient.PostAsync("api/Submissions", content);
        if (!response.IsSuccessStatusCode) throw new Exception(await response.Content.ReadAsStringAsync());

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }

    public async Task FinalizeAsync(Guid id, string userId)
    {
        await AddAuthHeader();
        var response = await _httpClient.PostAsync($"api/Submissions/{id}/finalize", null);

        if (response.IsSuccessStatusCode)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            using var authUpdateClient = new HttpClient();
            authUpdateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            await authUpdateClient.PostAsync($"https://localhost:7041/api/users/set-author-role/{userId}", null);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Finalize failed: {error}");
        }
    }

    public async Task<List<SubmissionListModel>> GetMySubmissionsAsync()
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<SubmissionListModel>>("api/Submissions") ?? new();
    }

    public async Task<List<SubmissionStatsModel>> GetEditorSubmissionsAsync()
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<SubmissionStatsModel>>("api/Submissions/all") ?? new();
    }

    public async Task<SubmissionDetailModel?> GetSubmissionDetailAsync(Guid id)
    {
        await AddAuthHeader();
        try { return await _httpClient.GetFromJsonAsync<SubmissionDetailModel>($"api/Submissions/{id}"); }
        catch { return null; }
    }
}