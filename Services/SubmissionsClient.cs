using Jarvis.Web.Models;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using System.Net.Http.Json;

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

    public async Task SubmitAsync(SubmissionModel model, IBrowserFile file)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

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

        for (int i = 0; i < model.Authors.Count; i++)
        {
            content.Add(new StringContent(model.Authors[i].FirstName), $"Authors[{i}].FirstName");
            content.Add(new StringContent(model.Authors[i].LastName), $"Authors[{i}].LastName");
            content.Add(new StringContent(model.Authors[i].Email), $"Authors[{i}].Email");
            content.Add(new StringContent(model.Authors[i].Affiliation), $"Authors[{i}].Affiliation");
            content.Add(new StringContent(model.Authors[i].Country), $"Authors[{i}].Country");
            content.Add(new StringContent(model.Authors[i].IsCorresponding.ToString()), $"Authors[{i}].IsCorresponding");
        }

        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 10));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "ManuscriptFile", file.Name);

        var response = await _httpClient.PostAsync("api/Submissions", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task<List<SubmissionListModel>> GetMySubmissionsAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await _httpClient.GetFromJsonAsync<List<SubmissionListModel>>("api/Submissions") ?? new();
    }

    public async Task<List<SubmissionStatsModel>> GetEditorSubmissionsAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return await _httpClient.GetFromJsonAsync<List<SubmissionStatsModel>>("api/Submissions/all") ?? new();
    }
}