using System.Net.Http.Json;
using System.Text.Json;

namespace StarterApp.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
{
    PropertyNameCaseInsensitive = true
};


    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"items/{id}");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<ItemDto>(stream, _jsonOptions);
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
{
    using var response = await _httpClient.GetAsync("categories");
    response.EnsureSuccessStatusCode();

    var categoriesResponse = await response.Content.ReadFromJsonAsync<CategoriesResponse>(_jsonOptions);
    return categoriesResponse?.Categories ?? new List<CategoryDto>();
}

public async Task<ItemDto?> CreateItemAsync(CreateItemRequest request)
{
    using var response = await _httpClient.PostAsJsonAsync("items", request, _jsonOptions);

    if (!response.IsSuccessStatusCode)
    {
        var errorText = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(errorText);
    }

    return await response.Content.ReadFromJsonAsync<ItemDto>(_jsonOptions);
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
