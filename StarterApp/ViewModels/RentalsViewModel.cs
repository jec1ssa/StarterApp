using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : BaseViewModel
{
    private readonly IRentalService _rentalService;

    public ObservableCollection<RentalDto> IncomingRentals { get; } = new();
    public ObservableCollection<RentalDto> OutgoingRentals { get; } = new();

    [ObservableProperty]
    private bool showIncoming;

    public RentalsViewModel(IRentalService rentalService)
    {
        _rentalService = rentalService;
        Title = "Rental Requests";
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
        IncomingRentals.Clear();
        OutgoingRentals.Clear();

        var incoming = await _rentalService.GetIncomingRentalsAsync();
        var outgoing = await _rentalService.GetOutgoingRentalsAsync();

        foreach (var rental in incoming)
            IncomingRentals.Add(rental);

        foreach (var rental in outgoing)
            OutgoingRentals.Add(rental);
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

            await updateStatusAsync(rental);
            await RefreshRentalsAsync();
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
}
