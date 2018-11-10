using KeyVendor.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConnectionPage : ContentPage
	{
		public ConnectionPage(ConnectionPageViewModel viewModel)
		{
			InitializeComponent();
            BindingContext = viewModel;
		}

        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            ((ConnectionPageViewModel)BindingContext).StopRefreshing();
        }
    }
}