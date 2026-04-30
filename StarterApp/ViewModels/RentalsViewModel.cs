using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;
    private readonly INavigationService _navigationService;
    private readonly List<RentalDto> _allIncomingRentals = new();
    private readonly List<RentalDto> _allOutgoingRentals = new();

    public ObservableCollection<RentalDto> IncomingRentals { get; } = new();
    public ObservableCollection<RentalDto> OutgoingRentals { get; } = new();
    public ObservableCollection<string> StatusFilters { get; } = new()
    {
        "All",
        RentalStatuses.Requested,
        RentalStatuses.Approved,
        RentalStatuses.OutForRent,
        RentalStatuses.Overdue,
        RentalStatuses.Returned,
        RentalStatuses.Completed,
        RentalStatuses.Rejected
    };

    [ObservableProperty]
    private bool showIncoming;

    [ObservableProperty]
    private string selectedStatusFilter = "All";

    [ObservableProperty]
    private string successMessage = "";

    [ObservableProperty]
    private bool hasSuccess;

    public RentalsViewModel(IRentalService rentalService, INavigationService navigationService)
    {
        _rentalService = rentalService;
        _navigationService = navigationService;
        Title = "Rental Requests";
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        ApplyFilters();
    }

    [RelayCommand]
    private async Task LoadRentalsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            await RefreshRentalsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to load rentals: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshRentalsAsync()
    {
        _allIncomingRentals.Clear();
        _allOutgoingRentals.Clear();

        var incoming = await _rentalService.GetIncomingRentalsAsync();
        var outgoing = await _rentalService.GetOutgoingRentalsAsync();

        _allIncomingRentals.AddRange(incoming);
        _allOutgoingRentals.AddRange(outgoing);

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        IncomingRentals.Clear();
        OutgoingRentals.Clear();

        foreach (var rental in FilterByStatus(_allIncomingRentals))
            IncomingRentals.Add(rental);

        foreach (var rental in FilterByStatus(_allOutgoingRentals))
            OutgoingRentals.Add(rental);
    }

    private IEnumerable<RentalDto> FilterByStatus(IEnumerable<RentalDto> rentals)
    {
        if (SelectedStatusFilter == "All")
            return rentals;

        return rentals.Where(rental => rental.Status == SelectedStatusFilter);
    }

    [RelayCommand]
    private void ShowIncomingRentals()
    {
        ShowIncoming = true;
    }

    [RelayCommand]
    private void ShowOutgoingRentals()
    {
        ShowIncoming = false;
    }

    [RelayCommand]
    private async Task ApproveRentalAsync(RentalDto rental)
    {
        await UpdateRentalAsync(rental, _rentalService.ApproveAsync, "Rental approved.");
    }

    [RelayCommand]
    private async Task RejectRentalAsync(RentalDto rental)
    {
        await UpdateRentalAsync(rental, _rentalService.RejectAsync, "Rental rejected.");
    }

    [RelayCommand]
    private async Task MarkOutForRentAsync(RentalDto rental)
    {
        await UpdateRentalAsync(rental, _rentalService.MarkOutForRentAsync, "Rental marked as out for rent.");
    }

    [RelayCommand]
    private async Task MarkReturnedAsync(RentalDto rental)
    {
        await UpdateRentalAsync(rental, _rentalService.MarkReturnedAsync, "Rental marked as returned.");
    }

    [RelayCommand]
    private async Task CompleteRentalAsync(RentalDto rental)
    {
        await UpdateRentalAsync(rental, _rentalService.CompleteAsync, "Rental completed.");
    }

    private async Task UpdateRentalAsync(
        RentalDto rental,
        Func<RentalDto, Task<RentalStatusUpdateDto?>> updateStatusAsync,
        string successMessage)
    {
        if (IsBusy || rental == null)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            HasSuccess = false;
            SuccessMessage = "";

            await updateStatusAsync(rental);
            await RefreshRentalsAsync();
            SuccessMessage = successMessage;
            HasSuccess = true;
        }
        catch (Exception ex)
        {
            SetError($"{successMessage.TrimEnd('.')} failed: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ReviewRentalAsync(RentalDto rental)
    {
        if (rental == null)
            return;

        if (!rental.CanReview)
        {
            SetError("Only completed rentals can be reviewed.");
            return;
        }

        var itemTitle = Uri.EscapeDataString(rental.ItemTitle);
        await _navigationService.NavigateToAsync(
            $"{nameof(StarterApp.Views.ReviewsPage)}?itemId={rental.ItemId}&rentalId={rental.Id}&itemTitle={itemTitle}");
    }
}
