using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Jarvis.Web.Components;
using Jarvis.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthService>(provider =>
    (IAuthService)provider.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped(sp => new HttpClient());

builder.Services.AddHttpClient<ReviewsClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7047");
});

builder.Services.AddHttpClient<SubmissionsClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7004");
});

builder.Services.AddHttpClient<SubmissionApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7004/");
});

builder.Services.AddHttpClient<VenueApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7190/");
});

builder.Services.AddHttpClient<UsersClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7041/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();