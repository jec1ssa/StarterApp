using StarterApp.Repositories;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Test.ViewModels;

public class NearbyItemsViewModelTests
{
    [Fact]
    public async Task SearchNearbyCommand_WhenLatitudeIsInvalid_ShowsLatitudeError()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Latitude = 91;

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Latitude must be between -90 and 90.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SearchNearbyCommand_WhenRadiusIsTooLarge_ShowsRadiusError()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.RadiusKm = 51;

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Radius must be between 1 and 50 km.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SearchNearbyCommand_WhenInputIsValid_LoadsNearbyItems()
    {
        // Arrange
        var repository = new FakeItemRepository();
        var viewModel = CreateViewModel(repository);

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.HasError);
        Assert.Single(viewModel.Items);
        Assert.Equal(5, repository.LastRadiusKm);
        Assert.Equal("1 item(s) within 5 km.", viewModel.ResultsSummary);
    }

    private static NearbyItemsViewModel CreateViewModel(FakeItemRepository? repository = null) =>
        new(
            repository ?? new FakeItemRepository(),
            new FakeLocationService(),
            new FakeNavigationService());

    private sealed class FakeItemRepository : IItemRepository
    {
        public double? LastRadiusKm { get; private set; }

        public Task<IReadOnlyList<ItemDto>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<ItemDto>>(Array.Empty<ItemDto>());

        public Task<ItemDto?> GetByIdAsync(int id) =>
            Task.FromResult<ItemDto?>(null);

        public Task<IReadOnlyList<ItemDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            double radiusKm,
            string? categorySlug = null)
        {
            LastRadiusKm = radiusKm;
            return Task.FromResult<IReadOnlyList<ItemDto>>(new[]
            {
                new ItemDto { Id = 1, Title = "Tent", Distance = 2.4 }
            });
        }

        public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
            Task.FromResult<IReadOnlyList<CategoryDto>>(Array.Empty<CategoryDto>());

        public Task<ItemDto?> CreateAsync(CreateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);

        public Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);
    }

    private sealed class FakeLocationService : ILocationService
    {
        public Task<LocationCoordinates> GetCurrentLocationAsync() =>
            Task.FromResult(new LocationCoordinates(55.9533, -3.1883));
    }

    private sealed class FakeNavigationService : INavigationService
    {
        public Task NavigateToAsync(string route) => Task.CompletedTask;

        public Task NavigateToAsync(string route, Dictionary<string, object> parameters) =>
            Task.CompletedTask;

        public Task NavigateBackAsync() => Task.CompletedTask;

        public Task NavigateToRootAsync() => Task.CompletedTask;

        public Task PopToRootAsync() => Task.CompletedTask;
    }
}
