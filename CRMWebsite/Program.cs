using CRMWebsite.Components;
using CRMWebsite.Services;
using MudBlazor.Services;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// COMPONENTS AND CLIENTS
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpClient<CompaniesHouseService>((provider, client) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["CompaniesHouse:ApiKey"];
    var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKey + ":"));
    client.BaseAddress = new Uri("https://api.company-information.service.gov.uk");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
});



// SCOPED

// SINGLETON

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
