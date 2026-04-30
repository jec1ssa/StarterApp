using StarterApp.Services;

namespace StarterApp.Repositories;

public class ApiItemRepository : IItemRepository
{
    private readonly IApiService _apiService;

    public ApiItemRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IReadOnlyList<ItemDto>> GetAllAsync() =>
        await _apiService.GetItemsAsync();

    public Task<ItemDto?> GetByIdAsync(int id) =>
        _apiService.GetItemByIdAsync(id);

    public async Task<IReadOnlyList<ItemDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        string? categorySlug = null) =>
        await _apiService.GetNearbyItemsAsync(latitude, longitude, radiusKm, categorySlug);

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
        await _apiService.GetCategoriesAsync();

    public Task<ItemDto?> CreateAsync(CreateItemRequest request) =>
        _apiService.CreateItemAsync(request);

    public Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request) =>
        _apiService.UpdateItemAsync(id, request);
}
