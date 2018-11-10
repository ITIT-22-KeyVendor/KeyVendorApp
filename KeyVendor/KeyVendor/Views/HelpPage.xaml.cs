using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HelpPage : ContentPage
	{
		public HelpPage (HelpPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
		}
	}
}