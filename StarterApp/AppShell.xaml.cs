using Microsoft.Extensions.DependencyInjection;
using StarterApp.ViewModels;
using StarterApp.Views;

namespace StarterApp;

public partial class AppShell : Shell
{
	public AppShell(AppShellViewModel viewModel, IServiceProvider serviceProvider)
	{	
		InitializeComponent();
		BindingContext = viewModel;

		Items.Add(new ShellContent
		{
			Content = serviceProvider.GetRequiredService<ItemsListPage>()
		});
	}
}
