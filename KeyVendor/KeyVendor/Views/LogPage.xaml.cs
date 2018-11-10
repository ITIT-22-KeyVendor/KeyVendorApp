using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogPage : ContentPage
	{
        public LogPage(LogPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
        }
	}
}