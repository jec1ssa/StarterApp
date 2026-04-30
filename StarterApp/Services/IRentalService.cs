namespace StarterApp.Services;

public interface IRentalService
{
    Task<List<RentalDto>> GetIncomingRentalsAsync();
    Task<List<RentalDto>> GetOutgoingRentalsAsync();
    Task<RentalStatusUpdateDto?> ApproveAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> RejectAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> MarkOutForRentAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> MarkReturnedAsync(RentalDto rental);
    Task<RentalStatusUpdateDto?> CompleteAsync(RentalDto rental);
}
