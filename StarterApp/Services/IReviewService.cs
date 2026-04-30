namespace StarterApp.Services;

public interface IReviewService
{
    Task<List<ReviewDto>> GetItemReviewsAsync(int itemId);
    Task<ReviewDto?> SubmitReviewAsync(int rentalId, int rating, string comment);
}
