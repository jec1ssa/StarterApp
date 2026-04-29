namespace StarterApp.Services;

public interface IApiService
{
    Task<List<ItemDto>> GetItemsAsync();
}