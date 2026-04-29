using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<ItemDto> Items { get; } = new();

    [ObservableProperty]
    private ItemDto? selectedItem;

    public ItemsListViewModel(IApiService apiService, INavigationService navigationService)
    {
        _apiService = apiService;
        _navigationService = navigationService;
        Title = "Rental Marketplace";
    }

    partial void OnSelectedItemChanged(ItemDto? value)
    {
        if (value == null)
            return;

        _ = GoToItemDetailAsync(value);
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task GoToItemDetailAsync(ItemDto item)
    {
        if (item == null)
            return;

        await _navigationService.NavigateToAsync($"{nameof(StarterApp.Views.ItemDetailPage)}?itemId={item.Id}");
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            Items.Clear();

            var items = await _apiService.GetItemsAsync();

            foreach (var item in items)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load items: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
private async Task GoToCreateItemAsync()
{
    await _navigationService.NavigateToAsync(nameof(StarterApp.Views.CreateItemPage));
}

}
