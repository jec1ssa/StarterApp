using StarterApp.Repositories;

namespace StarterApp.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<ReviewDto>> GetItemReviewsAsync(int itemId)
    {
        if (itemId <= 0)
            throw new ArgumentException("Item id must be valid.", nameof(itemId));

        return (await _reviewRepository.GetForItemAsync(itemId)).ToList();
    }

    public Task<ReviewDto?> SubmitReviewAsync(int rentalId, int rating, string comment)
    {
        if (rentalId <= 0)
            throw new ArgumentException("Rental id must be valid.", nameof(rentalId));

        if (rating is < 1 or > 5)
            throw new InvalidOperationException("Rating must be between 1 and 5.");

        if (comment.Length > 500)
            throw new InvalidOperationException("Comment must be 500 characters or fewer.");

        return _reviewRepository.CreateAsync(new CreateReviewRequest
        {
            RentalId = rentalId,
            Rating = rating,
            Comment = comment.Trim()
        });
    }
}
