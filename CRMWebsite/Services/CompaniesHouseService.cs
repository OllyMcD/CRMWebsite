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
    [JsonPropertyName("company_name")]
    public string? Name { get; set; }

    [JsonPropertyName("company_number")]
    public string? CompanyNumber { get; set; }

    [JsonPropertyName("registered_office_address")]
    public Address? RegisteredAddress { get; set; }

    [JsonPropertyName("company_status")]
    public string? Status { get; set; }

    [JsonPropertyName("company_type")]
    public string? CompanyType { get; set; }

    [JsonPropertyName("date_of_creation")]
    public DateTime? IncorporationDate { get; set; }

    [JsonPropertyName("sic_codes")]
    public List<string> IndustryCodes { get; set; } = new List<string>();

    [JsonPropertyName("website")]
    public string? Website { get; set; } // From filing history if available

    [JsonPropertyName("employees")]
    public int? EstimatedEmployees { get; set; } // From accounts if available

    [JsonPropertyName("description")] // Contains SIC codes and industry info
    public string? IndustryDescription { get; set; }

    [JsonPropertyName("charges")] // Contains charges
    public string? Charges { get; set; }
    
    [JsonPropertyName("uk_establishments")] // Contains establishments
    public string? UkEstablishments { get; set; }
}

public class Address
{
    [JsonPropertyName("address_line_1")]
    public string? Line1 { get; set; }

    [JsonPropertyName("address_line_2")]
    public string? Line2 { get; set; }

    [JsonPropertyName("locality")]
    public string? Town { get; set; }

    [JsonPropertyName("postal_code")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    public string FullAddress =>
        string.Join(", ", new[] { Line1, Line2, Town, Postcode, Country }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
}

