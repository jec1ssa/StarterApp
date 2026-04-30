using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Repositories;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;
    private readonly INavigationService _navigationService;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [ObservableProperty]
    private string titleInput = "";

    [ObservableProperty]
    private string description = "";

    [ObservableProperty]
    private decimal dailyRate;

    [ObservableProperty]
    private CategoryDto? selectedCategory;

    [ObservableProperty]
    private double latitude = 55.9533;

    [ObservableProperty]
    private double longitude = -3.1883;

    public CreateItemViewModel(IItemRepository itemRepository, INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _navigationService = navigationService;
        Title = "Create Item";
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        if (Categories.Count > 0)
            return;

        try
        {
            ClearError();

            var categories = await _itemRepository.GetCategoriesAsync();

            Categories.Clear();
            foreach (var category in categories)
                Categories.Add(category);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load categories: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
        if (IsBusy)
            return;

        if (string.IsNullOrWhiteSpace(TitleInput))
        {
            SetError("Title is required.");
            return;
        }

        if (TitleInput.Length < 5)
        {
            SetError("Title must be at least 5 characters.");
            return;
        }

        if (DailyRate <= 0)
        {
            SetError("Daily rate must be greater than 0.");
            return;
        }

        if (SelectedCategory == null)
        {
            SetError("Choose a category.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

            var request = new CreateItemRequest
            {
                Title = TitleInput.Trim(),
                Description = Description.Trim(),
                DailyRate = DailyRate,
                CategoryId = SelectedCategory.Id,
                Latitude = Latitude,
                Longitude = Longitude
            };

            await _itemRepository.CreateAsync(request);

            await _navigationService.NavigateBackAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to create item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}
