using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRMWebsite.Services;

public class CompaniesHouseService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.company-information.service.gov.uk";
    private readonly string _apiKey; // Your API key

    public CompaniesHouseService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        //Test
        //Console.WriteLine($"Companies House API Key: {_apiKey}");
        //Console.WriteLine($"Authorization Header: {_httpClient.DefaultRequestHeaders.Authorization}");
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

            // Debugging the full request
            //Console.WriteLine($"Request URL: {BaseUrl}{url}");

            var response = await _httpClient.GetAsync(url);

            // Debugging response status and content
            //Console.WriteLine($"Response Status: {response.StatusCode}");
            var content = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response Content: {content}");

            response.EnsureSuccessStatusCode();
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
    [JsonPropertyName("items_per_page")]
    public int ItemsPerPage { get; set; }
    
    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }

    [JsonPropertyName("items")]
    public List<CompanyItem> Items { get; set; }
}

public class CompanyItem
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("company_number")]
    public string? CompanyNumber { get; set; }

    [JsonPropertyName("company_status")]
    public string? CompanyStatus { get; set; }

    [JsonPropertyName("date_of_creation")]
    public DateTime DateOfCreation { get; set; }

    [JsonPropertyName("address_snippet")]
    public string? AddressSnippet { get; set; }
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