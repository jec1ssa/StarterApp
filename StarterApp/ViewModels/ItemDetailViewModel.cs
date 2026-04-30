using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Repositories;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemDetailViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;
    private readonly IRentalService _rentalService;
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;

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
    [NotifyPropertyChangedFor(nameof(EstimatedTotalPrice))]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EstimatedTotalPrice))]
    private DateTime endDate = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private string successMessage = "";

    [ObservableProperty]
    private bool hasSuccess;

    public decimal EstimatedTotalPrice
    {
        get
        {
            if (Item == null || StartDate.Date < DateTime.Today || EndDate.Date <= StartDate.Date)
                return 0;

            return _rentalService.CalculateTotalPrice(Item.DailyRate, StartDate, EndDate);
        }
    }

    public ItemDetailViewModel(
        IItemRepository itemRepository,
        IRentalService rentalService,
        IAuthenticationService authenticationService,
        INavigationService navigationService)
    {
        _itemRepository = itemRepository;
        _rentalService = rentalService;
        _authenticationService = authenticationService;
        _navigationService = navigationService;
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
            OnPropertyChanged(nameof(EstimatedTotalPrice));
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

        if (CanEditItem)
        {
            SetError("You cannot request to rent your own item.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            SuccessMessage = "";
            HasSuccess = false;

            var rental = await _rentalService.RequestRentalAsync(Item, StartDate, EndDate);

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

    [RelayCommand]
    private async Task ViewReviewsAsync()
    {
        if (Item == null)
        {
            SetError("Item details are still loading.");
            return;
        }

        var itemTitle = Uri.EscapeDataString(Item.Title);
        await _navigationService.NavigateToAsync(
            $"{nameof(StarterApp.Views.ReviewsPage)}?itemId={Item.Id}&itemTitle={itemTitle}");
    }

}
