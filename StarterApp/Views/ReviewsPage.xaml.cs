using StarterApp.ViewModels;

namespace StarterApp.Views;

public partial class ReviewsPage : ContentPage
{
    private readonly ReviewsViewModel _viewModel;

    public ReviewsPage(ReviewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadReviewsCommand.ExecuteAsync(null);
    }
}
