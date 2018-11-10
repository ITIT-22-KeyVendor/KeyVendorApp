using KeyVendor.ViewModels;
using Xamarin.Forms;

namespace KeyVendor.Views
{
    public partial class MainPage : ContentPage
	{
		public MainPage(MainPageViewModel viewModel)
		{
			InitializeComponent();
            BindingContext = viewModel;
		}
	}
}