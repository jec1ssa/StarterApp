using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<ItemDto> Items { get; } = new();

    public ItemsListViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Rental Marketplace";
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
}