using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProfilePage : ContentPage
	{
		public ProfilePage (ProfilePageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
        }
	}
}