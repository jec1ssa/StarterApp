using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Repositories;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class NearbyItemsViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;
    private readonly INavigationService _navigationService;

    public ObservableCollection<ItemDto> Items { get; } = new();
    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty]
    private double latitude = 55.9533;

    [ObservableProperty]
    private double longitude = -3.1883;

    [ObservableProperty]
    private double radiusKm = 5;

    [ObservableProperty]
    private CategoryDto? selectedCategory;

    [ObservableProperty]
    private string resultsSummary = "No search run yet.";

    public NearbyItemsViewModel(
        IItemRepository itemRepository,
        ILocationService locationService,
        INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _locationService = locationService;
        _navigationService = navigationService;
        Title = "Nearby Items";
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        if (Categories.Count > 0)
            return;

        try
        {
            ClearError();
            Categories.Clear();
            Categories.Add(new CategoryDto { Name = "All categories", Slug = "" });

            var categories = await _itemRepository.GetCategoriesAsync();

            foreach (var category in categories)
                Categories.Add(category);

            SelectedCategory = Categories.FirstOrDefault();
        }
        catch (Exception ex)
        {
            SetError($"Failed to load categories: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task UseCurrentLocationAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var location = await _locationService.GetCurrentLocationAsync();
            Latitude = location.Latitude;
            Longitude = location.Longitude;
        }
        catch (Exception ex)
        {
            SetError($"Failed to get current location: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (IsBusy)
            return;

        if (Latitude is < -90 or > 90)
        {
            SetError("Latitude must be between -90 and 90.");
            return;
        }

        if (Longitude is < -180 or > 180)
        {
            SetError("Longitude must be between -180 and 180.");
            return;
        }

        if (RadiusKm <= 0 || RadiusKm > 50)
        {
            SetError("Radius must be between 1 and 50 km.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            Items.Clear();

            var categorySlug = string.IsNullOrWhiteSpace(SelectedCategory?.Slug)
                ? null
                : SelectedCategory.Slug;

            var items = await _itemRepository.GetNearbyAsync(Latitude, Longitude, RadiusKm, categorySlug);

            foreach (var item in items)
                Items.Add(item);

            ResultsSummary = $"{Items.Count} item(s) within {RadiusKm:F0} km.";
        }
        catch (Exception ex)
        {
            SetError($"Failed to search nearby items: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToItemDetailAsync(ItemDto item)
    {
        if (item == null)
            return;

        await _navigationService.NavigateToAsync($"{nameof(StarterApp.Views.ItemDetailPage)}?itemId={item.Id}");
    }
}
