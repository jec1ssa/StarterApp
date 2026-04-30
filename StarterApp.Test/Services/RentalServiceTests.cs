using StarterApp.Repositories;
using StarterApp.Services;

namespace StarterApp.Test.Services;

public class RentalServiceTests
{
    [Fact]
    public async Task ApproveAsync_WhenRentalRequested_UpdatesStatusToApproved()
    {
        // Arrange
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);
        var rental = new RentalDto { Id = 12, Status = RentalStatuses.Requested };

        // Act
        var result = await service.ApproveAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RentalStatuses.Approved, result.Status);
        Assert.Equal(RentalStatuses.Approved, rental.Status);
        Assert.Equal(RentalStatuses.Approved, repository.LastRequestedStatus);
    }

    [Fact]
    public async Task RejectAsync_WhenRentalApproved_ThrowsInvalidOperationException()
    {
        // Arrange
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);
        var rental = new RentalDto { Id = 12, Status = RentalStatuses.Approved };

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RejectAsync(rental));

        // Assert
        Assert.Contains("Cannot change rental", exception.Message);
        Assert.Null(repository.LastRequestedStatus);
    }

    [Theory]
    [InlineData("2026-05-01", "2026-05-04", 7.50, 22.50)]
    [InlineData("2026-05-10", "2026-05-11", 12.00, 12.00)]
    public void CalculateTotalPrice_WhenDatesAreValid_ReturnsDailyRateTimesNumberOfDays(
        string start,
        string end,
        decimal dailyRate,
        decimal expectedTotal)
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());

        // Act
        var total = service.CalculateTotalPrice(
            dailyRate,
            DateTime.Parse(start),
            DateTime.Parse(end));

        // Assert
        Assert.Equal(expectedTotal, total);
    }

    [Fact]
    public async Task RequestRentalAsync_WhenEndDateBeforeStartDate_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());
        var item = new ItemDto { Id = 7, OwnerId = 3 };
        var startDate = DateTime.Today.AddDays(3);
        var endDate = DateTime.Today.AddDays(1);

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RequestRentalAsync(item, startDate, endDate));

        // Assert
        Assert.Equal("End date must be after start date.", exception.Message);
    }

    private sealed class FakeRentalRepository : IRentalRepository
    {
        public string? LastRequestedStatus { get; private set; }

        public Task<IReadOnlyList<RentalDto>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<RentalDto>>(Array.Empty<RentalDto>());

        public Task<RentalDto?> GetByIdAsync(int id) =>
            Task.FromResult<RentalDto?>(null);

        public Task<IReadOnlyList<RentalDto>> GetIncomingAsync() =>
            Task.FromResult<IReadOnlyList<RentalDto>>(Array.Empty<RentalDto>());

        public Task<IReadOnlyList<RentalDto>> GetOutgoingAsync() =>
            Task.FromResult<IReadOnlyList<RentalDto>>(Array.Empty<RentalDto>());

        public Task<RentalDto?> CreateAsync(CreateRentalRequest request) =>
            Task.FromResult<RentalDto?>(new RentalDto
            {
                ItemId = request.ItemId,
                Status = RentalStatuses.Requested,
                StartDate = DateTime.Parse(request.StartDate),
                EndDate = DateTime.Parse(request.EndDate)
            });

        public Task<RentalStatusUpdateDto?> UpdateStatusAsync(
            int rentalId,
            UpdateRentalStatusRequest request)
        {
            LastRequestedStatus = request.Status;

            return Task.FromResult<RentalStatusUpdateDto?>(new RentalStatusUpdateDto
            {
                Id = rentalId,
                Status = request.Status,
                UpdatedAt = DateTime.UtcNow
            });
        }
    }
}
