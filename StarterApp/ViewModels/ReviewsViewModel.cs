using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
[QueryProperty(nameof(RentalId), "rentalId")]
[QueryProperty(nameof(ItemTitle), "itemTitle")]
public partial class ReviewsViewModel : BaseViewModel
{
    private readonly IReviewService _reviewService;

    public ObservableCollection<ReviewDto> Reviews { get; } = new();

    [ObservableProperty]
    private int itemId;

    [ObservableProperty]
    private int rentalId;

    [ObservableProperty]
    private string itemTitle = "";

    [ObservableProperty]
    private int rating = 5;

    [ObservableProperty]
    private string comment = "";

    [ObservableProperty]
    private string successMessage = "";

    [ObservableProperty]
    private bool hasSuccess;

    [ObservableProperty]
    private bool canSubmitReview;

    public ReviewsViewModel(IReviewService reviewService)
    {
        _reviewService = reviewService;
        Title = "Reviews";
    }

    partial void OnItemIdChanged(int value)
    {
        _ = LoadReviewsAsync();
    }

    partial void OnRentalIdChanged(int value)
    {
        CanSubmitReview = value > 0;
    }

    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        if (IsBusy || ItemId <= 0)
            return;

        try
        {
            IsBusy = true;
            ClearError();
            Reviews.Clear();

            var reviews = await _reviewService.GetItemReviewsAsync(ItemId);

            foreach (var review in reviews)
                Reviews.Add(review);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load reviews: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if (IsBusy)
            return;

        if (!CanSubmitReview)
        {
            SetError("A completed rental is required before leaving a review.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            HasSuccess = false;
            SuccessMessage = "";

            var review = await _reviewService.SubmitReviewAsync(RentalId, Rating, Comment);

            if (review != null)
                Reviews.Insert(0, review);

            Comment = "";
            Rating = 5;
            SuccessMessage = "Review submitted.";
            HasSuccess = true;
        }
        catch (Exception ex)
        {
            SetError($"Failed to submit review: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
