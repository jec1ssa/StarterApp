namespace StarterApp.Services;

public interface IApiService
{
    Task<List<ItemDto>> GetItemsAsync();
    Task<List<ItemDto>> GetNearbyItemsAsync(double latitude, double longitude, double radiusKm, string? categorySlug = null);
    Task<ItemDto?> GetItemByIdAsync(int id);
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<ItemDto?> CreateItemAsync(CreateItemRequest request);
    Task<RentalDto?> CreateRentalAsync(CreateRentalRequest request);
    Task<List<RentalDto>> GetIncomingRentalsAsync();
    Task<List<RentalDto>> GetOutgoingRentalsAsync();
    Task<RentalStatusUpdateDto?> UpdateRentalStatusAsync(int rentalId, UpdateRentalStatusRequest request);
}
