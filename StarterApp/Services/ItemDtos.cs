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
    public decimal? AverageRating { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}
