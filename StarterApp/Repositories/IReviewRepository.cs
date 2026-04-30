using StarterApp.Services;

namespace StarterApp.Repositories;

public interface IReviewRepository : IRepository<ReviewDto>
{
    Task<IReadOnlyList<ReviewDto>> GetForItemAsync(int itemId);
    Task<ReviewDto?> CreateAsync(CreateReviewRequest request);
}
