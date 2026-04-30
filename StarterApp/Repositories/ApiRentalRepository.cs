using StarterApp.Services;

namespace StarterApp.Repositories;

public class ApiRentalRepository : IRentalRepository
{
    private readonly IApiService _apiService;

    public ApiRentalRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<IReadOnlyList<RentalDto>> GetAllAsync() =>
        throw new NotSupportedException("Rentals must be loaded as incoming or outgoing for the current user.");

    public Task<RentalDto?> GetByIdAsync(int id) =>
        _apiService.GetRentalByIdAsync(id);

    public async Task<IReadOnlyList<RentalDto>> GetIncomingAsync() =>
        await _apiService.GetIncomingRentalsAsync();

    public async Task<IReadOnlyList<RentalDto>> GetOutgoingAsync() =>
        await _apiService.GetOutgoingRentalsAsync();

    public Task<RentalDto?> CreateAsync(CreateRentalRequest request) =>
        _apiService.CreateRentalAsync(request);

    public Task<RentalStatusUpdateDto?> UpdateStatusAsync(int rentalId, UpdateRentalStatusRequest request) =>
        _apiService.UpdateRentalStatusAsync(rentalId, request);
}
