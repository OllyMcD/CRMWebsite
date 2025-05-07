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
    }

    public async Task<CompanySearchResult> SearchCompaniesAsync(string query,
        string location = null, int itemsPerPage = 60)
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
            return JsonSerializer.Deserialize<CompanySearchResult>(content);
        }
        catch (HttpRequestException ex)
        {
            // Log API errors
            Console.WriteLine($"API Error: {ex.Message}");
            throw new ApplicationException("Failed to search companies. Please try again later.", ex);
        }
        catch (JsonException ex)
        {
            // Log JSON parsing errors
            Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            throw new ApplicationException("Failed to process company data. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            // Log any other unexpected errors
            Console.WriteLine($"Unexpected Error: {ex.Message}");
            throw new ApplicationException("An unexpected error occurred. Please try again later.", ex);
        }
    }

    public async Task<CompanyProfile> GetCompanyProfileAsync(string companyNumber)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/company/{companyNumber}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CompanyProfile>(content);
        }
        catch (HttpRequestException ex)
        {
            // Log API errors
            Console.WriteLine($"API Error: {ex.Message}");
            throw new ApplicationException($"Failed to retrieve profile for company {companyNumber}. Please try again later.", ex);
        }
        catch (JsonException ex)
        {
            // Log JSON parsing errors
            Console.WriteLine($"JSON Parsing Error: {ex.Message}");
            throw new ApplicationException("Failed to process company profile data. Please try again later.", ex);
        }
        catch (Exception ex)
        {
            // Log any other unexpected errors
            Console.WriteLine($"Unexpected Error: {ex.Message}");
            throw new ApplicationException("An unexpected error occurred. Please try again later.", ex);
        }
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
    public DateTime? DateOfCreation { get; set; }

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
    public DateTime? IncorporationDate { get; set; }
}