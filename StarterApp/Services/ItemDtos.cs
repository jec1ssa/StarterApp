namespace StarterApp.Services;

public class ItemsResponse
{
    public List<ItemDto> Items { get; set; } = new();
}

public class NearbyItemsResponse
{
    public List<ItemDto> Items { get; set; } = new();
    public SearchLocationDto? SearchLocation { get; set; }
    public double Radius { get; set; }
    public int TotalResults { get; set; }
}

public class SearchLocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
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
    public double? Distance { get; set; }
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

public class CreateRentalRequest
{
    public int ItemId { get; set; }
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
}

public class RentalDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = "";
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = "";
    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "";
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool CanApprove => Status == RentalStatuses.Requested;
    public bool CanReject => Status == RentalStatuses.Requested;
    public bool CanMarkOutForRent => Status == RentalStatuses.Approved;
    public bool CanMarkReturned => Status == RentalStatuses.OutForRent || Status == RentalStatuses.Overdue;
    public bool CanComplete => Status == RentalStatuses.Returned;
}

public class RentalsResponse
{
    public List<RentalDto> Rentals { get; set; } = new();
    public int TotalRentals { get; set; }
}

public class UpdateRentalStatusRequest
{
    public string Status { get; set; } = "";
}

public class RentalStatusUpdateDto
{
    public int Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime UpdatedAt { get; set; }
}

public static class RentalStatuses
{
    public const string Requested = "Requested";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string OutForRent = "Out for Rent";
    public const string Overdue = "Overdue";
    public const string Returned = "Returned";
    public const string Completed = "Completed";
}
