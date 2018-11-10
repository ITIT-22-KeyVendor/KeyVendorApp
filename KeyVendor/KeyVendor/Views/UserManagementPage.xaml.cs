using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserManagementPage : ContentPage
	{
		public UserManagementPage (UserManagementPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
        }
	}
}