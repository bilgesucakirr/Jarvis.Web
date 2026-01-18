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

    public SubmissionsClient(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        this._localStorage = localStorage;
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
        content.Add(new StringContent(model.SubmitterEmail), "SubmitterEmail");
        content.Add(new StringContent(model.SubmitterName), "SubmitterName");
        if (!string.IsNullOrEmpty(model.OrganizerEmail)) content.Add(new StringContent(model.OrganizerEmail), "OrganizerEmail");
        content.Add(new StringContent(model.IsOriginal.ToString()), "IsOriginal");
        content.Add(new StringContent(model.IsNotElsewhere.ToString()), "IsNotElsewhere");
        content.Add(new StringContent(model.HasConsent.ToString()), "HasConsent");

        for (int i = 0; i < model.Authors.Count; i++)
        {
            content.Add(new StringContent(model.Authors[i].FirstName), $"Authors[{i}].FirstName");
            content.Add(new StringContent(model.Authors[i].LastName), $"Authors[{i}].LastName");
            content.Add(new StringContent(model.Authors[i].Email), $"Authors[{i}].Email");
            content.Add(new StringContent(model.Authors[i].Affiliation), $"Authors[{i}].Affiliation");
            content.Add(new StringContent(model.Authors[i].Country), $"Authors[{i}].Country");
            content.Add(new StringContent(model.Authors[i].IsCorresponding.ToString()), $"Authors[{i}].IsCorresponding");
        }

        var fileContent = new StreamContent(file.OpenReadStream(1024 * 1024 * 15));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        content.Add(fileContent, "ManuscriptFile", file.Name);

        var response = await _httpClient.PostAsync("api/Submissions", content);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("id").GetGuid();
    }

    public async Task FinalizeAsync(Guid id, string userId)
    {
        await AddAuthHeader();
        await _httpClient.PostAsync($"api/Submissions/{id}/finalize", null);
        var token = await _localStorage.GetItemAsync<string>("authToken");
        using var authUpdateClient = new HttpClient();
        authUpdateClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        await authUpdateClient.PostAsync($"https://localhost:7041/api/users/set-author-role/{userId}", null);
    }

    public async Task<List<SubmissionListModel>> GetMySubmissionsAsync()
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<SubmissionListModel>>("api/Submissions") ?? new();
    }

    public async Task<SubmissionDetailModel?> GetSubmissionDetailAsync(Guid id)
    {
        await AddAuthHeader();
        return await _httpClient.GetFromJsonAsync<SubmissionDetailModel>($"api/Submissions/{id}");
    }

    // TEK BİR TANE KALACAK ŞEKİLDE GÜNCELLENDİ
    public async Task<List<SubmissionStatsModel>> GetEditorSubmissionsAsync(Guid? venueId = null)
    {
        await AddAuthHeader();
        var url = venueId.HasValue && venueId.Value != Guid.Empty
                  ? $"api/Submissions/all?venueId={venueId.Value}"
                  : "api/Submissions/all";

        return await _httpClient.GetFromJsonAsync<List<SubmissionStatsModel>>(url) ?? new();
    }

    public async Task UploadRevisionAsync(Guid submissionId, IBrowserFile file, string type)
    {
        await AddAuthHeader();
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(15 * 1024 * 1024));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        content.Add(fileContent, "file", file.Name);
        await _httpClient.PostAsync($"api/Submissions/{submissionId}/upload-revision?type={type}", content);
    }

    public async Task RecordDecisionAsync(Guid submissionId, string decision, string decisionLetter)
    {
        await AddAuthHeader();
        int decisionValue = decision switch { "Accepted" => 7, "Rejected" => 8, "MinorRevisionRequired" => 3, "MajorRevisionRequired" => 4, _ => 0 };
        var payload = new { SubmissionId = submissionId, Decision = decisionValue, DecisionLetter = decisionLetter, NotifyAuthor = true };
        await _httpClient.PostAsJsonAsync($"api/Submissions/{submissionId}/decision", payload);
    }
}