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

    [Fact]
    public async Task RequestRentalAsync_WhenOwnerIsMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());
        var item = new ItemDto { Id = 7, OwnerId = 0 };

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RequestRentalAsync(item, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2)));

        // Assert
        Assert.Equal("Item owner details are required before requesting a rental.", exception.Message);
    }

    [Fact]
    public async Task RequestRentalAsync_WhenStartDateIsInPast_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());
        var item = new ItemDto { Id = 7, OwnerId = 3 };

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RequestRentalAsync(item, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)));

        // Assert
        Assert.Equal("Start date cannot be in the past.", exception.Message);
    }

    [Fact]
    public async Task RequestRentalAsync_WhenInputIsValid_CreatesRequestedRental()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());
        var item = new ItemDto { Id = 7, OwnerId = 3 };
        var start = DateTime.Today.AddDays(1);
        var end = DateTime.Today.AddDays(3);

        // Act
        var rental = await service.RequestRentalAsync(item, start, end);

        // Assert
        Assert.NotNull(rental);
        Assert.Equal(7, rental.ItemId);
        Assert.Equal(RentalStatuses.Requested, rental.Status);
    }

    [Fact]
    public void CalculateTotalPrice_WhenStartDateIsInPast_ThrowsInvalidOperationException()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CalculateTotalPrice(5, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1)));

        // Assert
        Assert.Equal("Start date cannot be in the past.", exception.Message);
    }

    [Fact]
    public async Task MarkOutForRentAsync_WhenRentalApproved_UpdatesStatus()
    {
        // Arrange
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);
        var rental = new RentalDto { Id = 15, Status = RentalStatuses.Approved };

        // Act
        var result = await service.MarkOutForRentAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RentalStatuses.OutForRent, rental.Status);
        Assert.Equal(RentalStatuses.OutForRent, repository.LastRequestedStatus);
    }

    [Theory]
    [InlineData(RentalStatuses.OutForRent)]
    [InlineData(RentalStatuses.Overdue)]
    public async Task MarkReturnedAsync_WhenStatusIsValid_UpdatesStatus(string currentStatus)
    {
        // Arrange
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);
        var rental = new RentalDto { Id = 16, Status = currentStatus };

        // Act
        var result = await service.MarkReturnedAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RentalStatuses.Returned, rental.Status);
        Assert.Equal(RentalStatuses.Returned, repository.LastRequestedStatus);
    }

    [Fact]
    public async Task CompleteAsync_WhenReturned_UpdatesStatus()
    {
        // Arrange
        var repository = new FakeRentalRepository();
        var service = new RentalService(repository);
        var rental = new RentalDto { Id = 17, Status = RentalStatuses.Returned };

        // Act
        var result = await service.CompleteAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(RentalStatuses.Completed, rental.Status);
        Assert.Equal(RentalStatuses.Completed, repository.LastRequestedStatus);
    }

    [Fact]
    public async Task GetIncomingRentalsAsync_ReturnsRepositoryItems()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());

        // Act
        var rentals = await service.GetIncomingRentalsAsync();

        // Assert
        Assert.NotNull(rentals);
    }

    [Fact]
    public async Task GetOutgoingRentalsAsync_ReturnsRepositoryItems()
    {
        // Arrange
        var service = new RentalService(new FakeRentalRepository());

        // Act
        var rentals = await service.GetOutgoingRentalsAsync();

        // Assert
        Assert.NotNull(rentals);
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
