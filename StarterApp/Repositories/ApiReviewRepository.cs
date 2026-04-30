using StarterApp.Services;

namespace StarterApp.Repositories;

public class ApiReviewRepository : IReviewRepository
{
    private readonly IApiService _apiService;

    public ApiReviewRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    public Task<IReadOnlyList<ReviewDto>> GetAllAsync() =>
        throw new NotSupportedException("Reviews must be loaded for a specific item.");

    public Task<ReviewDto?> GetByIdAsync(int id) =>
        throw new NotSupportedException("The shared API does not expose review lookup by id.");

    public async Task<IReadOnlyList<ReviewDto>> GetForItemAsync(int itemId) =>
        await _apiService.GetItemReviewsAsync(itemId);

    public Task<ReviewDto?> CreateAsync(CreateReviewRequest request) =>
        _apiService.CreateReviewAsync(request);
}
