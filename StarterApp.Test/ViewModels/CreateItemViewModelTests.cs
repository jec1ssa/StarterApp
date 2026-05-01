using StarterApp.Repositories;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Test.ViewModels;

public class CreateItemViewModelTests
{
    [Fact]
    public async Task CreateItemCommand_WhenTitleIsEmpty_ShowsTitleRequiredError()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService());

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Title is required.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItemCommand_WhenDailyRateIsZero_ShowsDailyRateError()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService())
        {
            TitleInput = "Cordless Drill",
            DailyRate = 0
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Daily rate must be greater than 0.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItemCommand_WhenInputIsValid_CreatesItemAndNavigatesBack()
    {
        // Arrange
        var repository = new FakeItemRepository();
        var navigation = new FakeNavigationService();
        var viewModel = new CreateItemViewModel(repository, navigation)
        {
            TitleInput = "Cordless Drill",
            Description = "Useful DIY drill",
            DailyRate = 8,
            SelectedCategory = new CategoryDto { Id = 1, Name = "Tools", Slug = "tools" }
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.HasError);
        Assert.NotNull(repository.LastCreateRequest);
        Assert.Equal("Cordless Drill", repository.LastCreateRequest.Title);
        Assert.True(navigation.NavigatedBack);
    }

    [Fact]
    public async Task CreateItemCommand_WhenTitleTooShort_ShowsValidationError()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService())
        {
            TitleInput = "Dril",
            DailyRate = 8
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Title must be at least 5 characters.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItemCommand_WhenCategoryMissing_ShowsValidationError()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService())
        {
            TitleInput = "Cordless Drill",
            DailyRate = 8
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.Equal("Choose a category.", viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CreateItemCommand_WhenRepositoryThrows_SetsError()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new ThrowingItemRepository(), new FakeNavigationService())
        {
            TitleInput = "Cordless Drill",
            Description = "Useful DIY drill",
            DailyRate = 8,
            SelectedCategory = new CategoryDto { Id = 1, Name = "Tools", Slug = "tools" }
        };

        // Act
        await viewModel.CreateItemCommand.ExecuteAsync(null);

        // Assert
        Assert.True(viewModel.HasError);
        Assert.StartsWith("Failed to create item:", viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task LoadCategoriesCommand_WhenCalled_PopulatesCategories()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService());

        // Act
        await viewModel.LoadCategoriesCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(viewModel.Categories);
        Assert.Equal("Tools", viewModel.Categories[0].Name);
    }

    [Fact]
    public async Task LoadCategoriesCommand_WhenAlreadyLoaded_DoesNotDuplicate()
    {
        // Arrange
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), new FakeNavigationService());
        await viewModel.LoadCategoriesCommand.ExecuteAsync(null);

        // Act
        await viewModel.LoadCategoriesCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(viewModel.Categories);
    }

    [Fact]
    public async Task CancelCommand_NavigatesBack()
    {
        // Arrange
        var navigation = new FakeNavigationService();
        var viewModel = new CreateItemViewModel(new FakeItemRepository(), navigation);

        // Act
        await viewModel.CancelCommand.ExecuteAsync(null);

        // Assert
        Assert.True(navigation.NavigatedBack);
    }

    private sealed class FakeItemRepository : IItemRepository
    {
        public CreateItemRequest? LastCreateRequest { get; private set; }

        public Task<IReadOnlyList<ItemDto>> GetAllAsync() =>
            Task.FromResult<IReadOnlyList<ItemDto>>(Array.Empty<ItemDto>());

        public Task<ItemDto?> GetByIdAsync(int id) =>
            Task.FromResult<ItemDto?>(null);

        public Task<IReadOnlyList<ItemDto>> GetNearbyAsync(
            double latitude,
            double longitude,
            double radiusKm,
            string? categorySlug = null) =>
            Task.FromResult<IReadOnlyList<ItemDto>>(Array.Empty<ItemDto>());

        public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
            Task.FromResult<IReadOnlyList<CategoryDto>>(new[]
            {
                new CategoryDto { Id = 1, Name = "Tools", Slug = "tools" }
            });

        public Task<ItemDto?> CreateAsync(CreateItemRequest request)
        {
            LastCreateRequest = request;
            return Task.FromResult<ItemDto?>(new ItemDto { Id = 10, Title = request.Title });
        }

        public Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);
    }

    private sealed class FakeNavigationService : INavigationService
    {
        public bool NavigatedBack { get; private set; }

        public Task NavigateToAsync(string route) => Task.CompletedTask;

        public Task NavigateToAsync(string route, Dictionary<string, object> parameters) =>
            Task.CompletedTask;

        public Task NavigateBackAsync()
        {
            NavigatedBack = true;
            return Task.CompletedTask;
        }

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
            Task.FromResult<IReadOnlyList<ItemDto>>(Array.Empty<ItemDto>());

        public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
            Task.FromResult<IReadOnlyList<CategoryDto>>(Array.Empty<CategoryDto>());

        public Task<ItemDto?> CreateAsync(CreateItemRequest request) =>
            throw new InvalidOperationException("Boom");

        public Task<ItemDto?> UpdateAsync(int id, UpdateItemRequest request) =>
            Task.FromResult<ItemDto?>(null);
    }
}
