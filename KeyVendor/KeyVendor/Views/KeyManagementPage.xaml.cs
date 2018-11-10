using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class KeyManagementPage : ContentPage
	{
		public KeyManagementPage (KeyManagementPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
        }
	}
}