using CommunityToolkit.Mvvm.ComponentModel;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemDetailViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int itemId;

    [ObservableProperty]
    private ItemDto? item;

    public ItemDetailViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Item Details";
    }

    partial void OnItemIdChanged(int value)
    {
        _ = LoadItemAsync(value);
    }

    private async Task LoadItemAsync(int id)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            Item = await _apiService.GetItemByIdAsync(id);

            if (Item == null)
                SetError("Item not found.");
        }
        catch (Exception ex)
        {
            SetError($"Failed to load item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}