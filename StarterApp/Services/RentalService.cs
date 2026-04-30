using StarterApp.Repositories;

namespace StarterApp.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;

    public RentalService(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<List<RentalDto>> GetIncomingRentalsAsync() =>
        (await _rentalRepository.GetIncomingAsync()).ToList();

    public async Task<List<RentalDto>> GetOutgoingRentalsAsync() =>
        (await _rentalRepository.GetOutgoingAsync()).ToList();

    public Task<RentalDto?> RequestRentalAsync(ItemDto item, DateTime startDate, DateTime endDate)
    {
        if (item.OwnerId <= 0)
            throw new InvalidOperationException("Item owner details are required before requesting a rental.");

        ValidateRentalDates(startDate, endDate);

        return _rentalRepository.CreateAsync(new CreateRentalRequest
        {
            ItemId = item.Id,
            StartDate = startDate.ToString("yyyy-MM-dd"),
            EndDate = endDate.ToString("yyyy-MM-dd")
        });
    }

    public decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate)
    {
        ValidateRentalDates(startDate, endDate);

        var days = (endDate.Date - startDate.Date).Days;
        return dailyRate * days;
    }

    public Task<RentalStatusUpdateDto?> ApproveAsync(RentalDto rental) =>
        UpdateStatusAsync(rental, RentalStatuses.Approved, RentalStatuses.Requested);

    public Task<RentalStatusUpdateDto?> RejectAsync(RentalDto rental) =>
        UpdateStatusAsync(rental, RentalStatuses.Rejected, RentalStatuses.Requested);

    public Task<RentalStatusUpdateDto?> MarkOutForRentAsync(RentalDto rental) =>
        UpdateStatusAsync(rental, RentalStatuses.OutForRent, RentalStatuses.Approved);

    public Task<RentalStatusUpdateDto?> MarkReturnedAsync(RentalDto rental) =>
        UpdateStatusAsync(rental, RentalStatuses.Returned, RentalStatuses.OutForRent, RentalStatuses.Overdue);

    public Task<RentalStatusUpdateDto?> CompleteAsync(RentalDto rental) =>
        UpdateStatusAsync(rental, RentalStatuses.Completed, RentalStatuses.Returned);

    private async Task<RentalStatusUpdateDto?> UpdateStatusAsync(
        RentalDto rental,
        string nextStatus,
        params string[] validCurrentStatuses)
    {
        if (!validCurrentStatuses.Contains(rental.Status))
            throw new InvalidOperationException($"Cannot change rental from {rental.Status} to {nextStatus}.");

        var result = await _rentalRepository.UpdateStatusAsync(
            rental.Id,
            new UpdateRentalStatusRequest { Status = nextStatus });

        if (result != null)
            rental.Status = result.Status;

        return result;
    }

    private static void ValidateRentalDates(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date < DateTime.Today)
            throw new InvalidOperationException("Start date cannot be in the past.");

        if (endDate.Date <= startDate.Date)
            throw new InvalidOperationException("End date must be after start date.");
    }
}
