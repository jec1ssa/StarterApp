namespace StarterApp.Services;

public interface IRentalService
{
    Task<List<RentalDto>> GetIncomingRentalsAsync();
    Task<List<RentalDto>> GetOutgoingRentalsAsync();
    Task<RentalDto?> RequestRentalAsync(ItemDto item, DateTime startDate, DateTime endDate);
    decimal CalculateTotalPrice(decimal dailyRate, DateTime startDate, DateTime endDate);
    Task<RentalStatusUpdateDto?> ApproveAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> RejectAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> MarkOutForRentAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> MarkReturnedAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> CompleteAsync(RentalDto rental);
}
