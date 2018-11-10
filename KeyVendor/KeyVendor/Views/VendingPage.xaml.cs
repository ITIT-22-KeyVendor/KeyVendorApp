using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VendingPage : ContentPage
	{
		public VendingPage (VendingPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
		}
	}
}