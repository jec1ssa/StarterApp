using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    public ObservableCollection<RentalDto> IncomingRentals { get; } = new();
    public ObservableCollection<RentalDto> OutgoingRentals { get; } = new();

    [ObservableProperty]
    private bool showIncoming;

    public RentalsViewModel(IApiService apiService)
    {
        _apiService = apiService;
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

            IncomingRentals.Clear();
            OutgoingRentals.Clear();

            var incoming = await _apiService.GetIncomingRentalsAsync();
            var outgoing = await _apiService.GetOutgoingRentalsAsync();

            foreach (var rental in incoming)
                IncomingRentals.Add(rental);

            foreach (var rental in outgoing)
                OutgoingRentals.Add(rental);
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
}
