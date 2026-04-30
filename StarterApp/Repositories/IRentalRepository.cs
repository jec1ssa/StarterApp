using StarterApp.Services;

namespace StarterApp.Repositories;

public interface IRentalRepository : IRepository<RentalDto>
{
    Task<IReadOnlyList<RentalDto>> GetIncomingAsync();
    Task<IReadOnlyList<RentalDto>> GetOutgoingAsync();
    Task<RentalDto?> CreateAsync(CreateRentalRequest request);
    Task<RentalStatusUpdateDto?> UpdateStatusAsync(int rentalId, UpdateRentalStatusRequest request);
}
