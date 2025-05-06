using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CRMWebsite.Services;

public class CompaniesHouseService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.company-information.service.gov.uk";
    private readonly string _apiKey; // Your API key

    public CompaniesHouseService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _apiKey = configuration["CompaniesHouse:ApiKey"]; // Get from appsettings.json

        // Correct authentication header format
        var authString = Convert.ToBase64String(Encoding.ASCII.GetBytes(_apiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authString);
    }

    public async Task<CompanySearchResult> SearchCompaniesAsync(string query,
        string location = null, int itemsPerPage = 10)
    {
        try
        {
            var url = $"/search/companies?q={Uri.EscapeDataString(query)}&items_per_page={itemsPerPage}";

            if (!string.IsNullOrEmpty(location))
            {
                url += $"&location={Uri.EscapeDataString(location)}";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Companies House API Key: {_apiKey}");
            return JsonSerializer.Deserialize<CompanySearchResult>(content);
        }
        catch (HttpRequestException ex)
        {
            // Handle API errors
            Console.WriteLine($"API Error: {ex.Message}");
            throw;
        }
    }

    public async Task<CompanyProfile> GetCompanyProfileAsync(string companyNumber)
    {
        var response = await _httpClient.GetAsync($"/company/{companyNumber}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<CompanyProfile>(content);
    }
}

// Model classes for the API responses
public class CompanySearchResult
{
    public int ItemsPerPage { get; set; }
    public int TotalResults { get; set; }
    public List<CompanySearchItem> Items { get; set; }
}

public class CompanySearchItem
{
    public string? CompanyNumber { get; set; }
    public string? Title { get; set; }
    public string? CompanyStatus { get; set; }
    public string? AddressSnippet { get; set; }
    public string? Description { get; set; }
    public DateTime DateOfCreation { get; set; }
}

public class CompanyProfile
{
    public string? CompanyName { get; set; }
    public string? CompanyNumber { get; set; }
    public string? RegisteredOfficeAddress { get; set; }
    public string? CompanyStatus { get; set; }
    public string? CompanyType { get; set; }
    public DateTime IncorporationDate { get; set; }
}