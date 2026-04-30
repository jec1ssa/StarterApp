using StarterApp.Services;

namespace StarterApp.Repositories;

public interface IItemRepository : IRepository<ItemDto>
{
    Task<IReadOnlyList<ItemDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm,
        string? categorySlug = null);

    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();
    Task<ItemDto?> CreateAsync(CreateItemRequest request);
    Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request);
}
