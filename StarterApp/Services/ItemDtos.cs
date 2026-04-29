namespace StarterApp.Services;

public class ItemsResponse
{
    public List<ItemDto> Items { get; set; } = new();
}

public class ItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; } = "";
    public string Location { get; set; } = "";
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = "";
    public decimal? OwnerRating { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
}

public class ReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public string ReviewerName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public int ItemCount { get; set; }
}

public class CategoriesResponse
{
    public List<CategoryDto> Categories { get; set; } = new();
}

public class CreateItemRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal DailyRate { get; set; }
    public int CategoryId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

