using StarterApp.Repositories;
using StarterApp.Services;

namespace StarterApp.Test.Services;

public class ReviewServiceTests
{
    [Fact]
    public async Task SubmitReviewAsync_WhenRatingIsValid_CreatesReview()
    {
        // Arrange
        var repository = new FakeReviewRepository();
        var service = new ReviewService(repository);

        // Act
        var review = await service.SubmitReviewAsync(42, 5, "Great item");

        // Assert
        Assert.NotNull(review);
        Assert.Equal(42, repository.LastRequest?.RentalId);
        Assert.Equal(5, repository.LastRequest?.Rating);
        Assert.Equal("Great item", repository.LastRequest?.Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task SubmitReviewAsync_WhenRatingIsOutsideRange_ThrowsInvalidOperationException(int rating)
    {
        // Arrange
        var service = new ReviewService(new FakeReviewRepository());

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SubmitReviewAsync(42, rating, "Comment"));

        // Assert
        Assert.Equal("Rating must be between 1 and 5.", exception.Message);
    }

    [Fact]
    public async Task GetItemReviewsAsync_WhenItemIdIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var service = new ReviewService(new FakeReviewRepository());

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetItemReviewsAsync(0));

        // Assert
        Assert.Contains("Item id must be valid.", exception.Message);
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenRentalIdIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var service = new ReviewService(new FakeReviewRepository());

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.SubmitReviewAsync(0, 5, "Comment"));

        // Assert
        Assert.Contains("Rental id must be valid.", exception.Message);
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenCommentTooLong_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new ReviewService(new FakeReviewRepository());
        var longComment = new string('x', 501);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SubmitReviewAsync(42, 5, longComment));

        // Assert
        Assert.Equal("Comment must be 500 characters or fewer.", exception.Message);
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenCommentHasWhitespace_TrimsBeforeCreate()
    {
        // Arrange
        var repository = new FakeReviewRepository();
        var service = new ReviewService(repository);

        // Act
        var review = await service.SubmitReviewAsync(42, 4, "  Great item  ");

        // Assert
        Assert.NotNull(review);
        Assert.Equal("Great item", repository.LastRequest?.Comment);
    }

    private sealed class FakeReviewRepository : IReviewRepository
    {
        public CreateReviewRequest? LastRequest { get; private set; }

        public Task<IReadOnlyList<ReviewDto>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<ReviewDto>>(Array.Empty<ReviewDto>());

        public Task<ReviewDto?> GetByIdAsync(int id) =>
            Task.FromResult<ReviewDto?>(null);

        public Task<IReadOnlyList<ReviewDto>> GetForItemAsync(int itemId) =>
            Task.FromResult<IReadOnlyList<ReviewDto>>(new[]
            {
                new ReviewDto { Id = 1, Rating = 5, Comment = "Helpful owner" }
            });

        public Task<ReviewDto?> CreateAsync(CreateReviewRequest request)
        {
            LastRequest = request;

            return Task.FromResult<ReviewDto?>(new ReviewDto
            {
                Id = 99,
                RentalId = request.RentalId,
                Rating = request.Rating,
                Comment = request.Comment
            });
        }
    }
}
