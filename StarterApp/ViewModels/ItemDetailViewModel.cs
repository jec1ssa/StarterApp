using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
private DateTime startDate = DateTime.Today;

[ObservableProperty]
private DateTime endDate = DateTime.Today.AddDays(1);

[ObservableProperty]
private string successMessage = "";

[ObservableProperty]
private bool hasSuccess;


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

    [RelayCommand]
private async Task RequestRentalAsync()
{
    if (IsBusy)
        return;

    if (Item == null)
    {
        SetError("Item details are still loading.");
        return;
    }

    if (StartDate.Date < DateTime.Today)
    {
        SetError("Start date cannot be in the past.");
        return;
    }

    if (EndDate.Date <= StartDate.Date)
    {
        SetError("End date must be after start date.");
        return;
    }

    try
    {
        IsBusy = true;
        ClearError();
        SuccessMessage = "";
        HasSuccess = false;

        var request = new CreateRentalRequest
        {
            ItemId = Item.Id,
            StartDate = StartDate.ToString("yyyy-MM-dd"),
            EndDate = EndDate.ToString("yyyy-MM-dd")
        };

        var rental = await _apiService.CreateRentalAsync(request);

        SuccessMessage = rental == null
            ? "Rental request submitted."
            : $"Rental request submitted. Status: {rental.Status}";

        HasSuccess = true;
    }
    catch (Exception ex)
    {
        SetError($"Failed to request rental: {ex.Message}");
    }
    finally
    {
        IsBusy = false;
    }
}

}