using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserPage : ContentPage
	{
		public UserPage (UserPageViewModel viewModel)
		{
			InitializeComponent();
            BindingContext = viewModel;
        }
	}
}