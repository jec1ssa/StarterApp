using System.Text.Json;

namespace StarterApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        using var response = await _httpClient.GetAsync("items");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<ItemDto>>(
                document.RootElement.GetRawText(),
                _jsonOptions) ?? new List<ItemDto>();
        }

        if (document.RootElement.TryGetProperty("items", out var itemsElement)
            && itemsElement.ValueKind == JsonValueKind.Array)
        {
            return JsonSerializer.Deserialize<List<ItemDto>>(
                itemsElement.GetRawText(),
                _jsonOptions) ?? new List<ItemDto>();
        }

        return new List<ItemDto>();
    }
}
