using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Repositories;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemDetailViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IAuthenticationService _authenticationService;

    [ObservableProperty]
    private int itemId;

    [ObservableProperty]
    private ItemDto? item;

    [ObservableProperty]
    private string editTitle = "";

    [ObservableProperty]
    private string editDescription = "";

    [ObservableProperty]
    private decimal editDailyRate;

    [ObservableProperty]
    private bool editIsAvailable;

    [ObservableProperty]
    private bool canEditItem;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string successMessage = "";

    [ObservableProperty]
    private bool hasSuccess;

    public ItemDetailViewModel(
        IItemRepository itemRepository,
        IRentalRepository rentalRepository,
        IAuthenticationService authenticationService)
    {
        _itemRepository = itemRepository;
        _rentalRepository = rentalRepository;
        _authenticationService = authenticationService;
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

            Item = await _itemRepository.GetByIdAsync(id);

            if (Item == null)
            {
                SetError("Item not found.");
                return;
            }

            EditTitle = Item.Title;
            EditDescription = Item.Description;
            EditDailyRate = Item.DailyRate;
            EditIsAvailable = Item.IsAvailable;
            CanEditItem = _authenticationService.CurrentUser?.Id == Item.OwnerId;
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

        if (!CanEditItem)
        {
            SetError("Only the owner can update this item.");
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

            var rental = await _rentalRepository.CreateAsync(request);

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

    [RelayCommand]
    private async Task SaveItemChangesAsync()
    {
        if (IsBusy)
            return;

        if (Item == null)
        {
            SetError("Item details are still loading.");
            return;
        }

        if (string.IsNullOrWhiteSpace(EditTitle) || EditTitle.Trim().Length < 5)
        {
            SetError("Title must be at least 5 characters.");
            return;
        }

        if (EditDailyRate <= 0)
        {
            SetError("Daily rate must be greater than 0.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            SuccessMessage = "";
            HasSuccess = false;

            var updatedItem = await _itemRepository.UpdateAsync(
                Item.Id,
                new UpdateItemRequest
                {
                    Title = EditTitle.Trim(),
                    Description = EditDescription.Trim(),
                    DailyRate = EditDailyRate,
                    IsAvailable = EditIsAvailable
                });

            if (updatedItem != null)
                Item = updatedItem;

            SuccessMessage = "Item changes saved.";
            HasSuccess = true;
        }
        catch (Exception ex)
        {
            SetError($"Failed to update item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

}
