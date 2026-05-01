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

    [Fact]
    public async Task SearchNearbyCommand_WhenLongitudeIsInvalid_ShowsLongitudeError()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Longitude = 181;

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Longitude must be between -180 and 180.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SearchNearbyCommand_WhenRepositoryThrows_SetsError()
    {
        // Arrange
        var viewModel = new NearbyItemsViewModel(
            new ThrowingItemRepository(),
            new FakeLocationService(),
            new FakeNavigationService());

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.StartsWith("Failed to search nearby items:", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task SearchNearbyCommand_WhenCategorySelected_UsesCategorySlug()
    {
        // Arrange
        var repository = new FakeItemRepository();
        var viewModel = CreateViewModel(repository);
        viewModel.SelectedCategory = new CategoryDto { Id = 1, Name = "Tools", Slug = "tools" };

        // Act
        await viewModel.SearchNearbyCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("tools", repository.LastCategorySlug);
    }

    [Fact]
    public async Task LoadCategoriesCommand_WhenCalled_AddsAllAndSelectsDefault()
    {
        // Arrange
        var viewModel = CreateViewModel();

        // Act
        await viewModel.LoadCategoriesCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.Categories.Count >= 1);
        Assert.Equal("All categories", viewModel.Categories[0].Name);
        Assert.NotNull(viewModel.SelectedCategory);
    }

    [Fact]
    public async Task UseCurrentLocationCommand_WhenCalled_UpdatesCoordinates()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.Latitude = 0;
        viewModel.Longitude = 0;

        // Act
        await viewModel.UseCurrentLocationCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(55.9533, viewModel.Latitude, 4);
        Assert.Equal(-3.1883, viewModel.Longitude, 4);
    }

    [Fact]
    public async Task GoToItemDetailCommand_WhenItemProvided_NavigatesToDetail()
    {
        // Arrange
        var navigation = new FakeNavigationService();
        var viewModel = new NearbyItemsViewModel(new FakeItemRepository(), new FakeLocationService(), navigation);
        var item = new ItemDto { Id = 44, Title = "Drill" };

        // Act
        await viewModel.GoToItemDetailCommand.ExecuteAsync(item);

        // Assert
        Assert.NotNull(navigation.LastRoute);
        Assert.Contains("itemId=44", navigation.LastRoute);
    }

    private static NearbyItemsViewModel CreateViewModel(FakeItemRepository? repository = null) =>
        new(
            repository ?? new FakeItemRepository(),
            new FakeLocationService(),
            new FakeNavigationService());

    private sealed class FakeItemRepository : IItemRepository
    {
        public double? LastRadiusKm { get; private set; }
        public string? LastCategorySlug { get; private set; }

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
            LastCategorySlug = categorySlug;
            return Task.FromResult<IReadOnlyList<ItemDto>>(new[]
            {
                new ItemDto { Id = 1, Title = "Tent", Distance = 2.4 }
            });
        }

        public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
            Task.FromResult<IReadOnlyList<CategoryDto>>(new[]
            {
                new CategoryDto { Id = 1, Name = "Tools", Slug = "tools" }
            });

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
        public string? LastRoute { get; private set; }

        public Task NavigateToAsync(string route)
        {
            LastRoute = route;
            return Task.CompletedTask;
        }

        public Task NavigateToAsync(string route, Dictionary<string, object> parameters) =>
            Task.CompletedTask;

        public Task NavigateBackAsync() => Task.CompletedTask;

        public Task NavigateToRootAsync() => Task.CompletedTask;

        public Task PopToRootAsync() => Task.CompletedTask;
    }

    private sealed class ThrowingItemRepository : IItemRepository
    {
        public Task<IReadOnlyList<ItemDto>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<ItemDto>>(Array.Empty<ItemDto>());

        public Task<ItemDto?> GetByIdAsync(int id) =>
            Task.FromResult<ItemDto?>(null);

        public Task<IReadOnlyList<ItemDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            double radiusKm,
            string? categorySlug = null) =>
            throw new InvalidOperationException("Boom");

        public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
            Task.FromResult<IReadOnlyList<CategoryDto>>(Array.Empty<CategoryDto>());

        public Task<ItemDto?> CreateAsync(CreateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);

        public Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);
    }
}
