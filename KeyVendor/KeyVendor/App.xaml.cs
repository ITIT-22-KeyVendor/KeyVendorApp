using KeyVendor.ViewModels;
using KeyVendor.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace KeyVendor
{
    public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            mainViewModel = new MainPageViewModel();
            mainViewModel.RestoreState(Current.Properties);
            mainViewModel.OnOpenConnectionPage += OpenConnectionPage;
            mainViewModel.OnOpenProfilePage += OpenProfilePage;
            mainViewModel.OnOpenHelpPage += OpenHelpPage;
            mainViewModel.OnOpenVendingPage += OpenVendingPage;

            MainPage = new NavigationPage(new MainPage(mainViewModel));
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}
		protected override void OnSleep ()
		{
            mainViewModel.SaveState(Current.Properties);
            mainViewModel.OnSleep();
        }
		protected override void OnResume ()
		{
            mainViewModel.OnResume();
		}

        private void OpenConnectionPage(object sender, ConnectionPageViewModel viewModel)
        {
            var page = new ConnectionPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenProfilePage(object sender, ProfilePageViewModel viewModel)
        {
            var page = new ProfilePage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenHelpPage(object sender, HelpPageViewModel viewModel)
        {
            var page = new HelpPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenVendingPage(object sender, VendingPageViewModel viewModel)
        {
            viewModel.OnOpenKeyManagementPage += OpenKeyManagementPage;
            viewModel.OnOpenUserManagementPage += OpenUserManagementPage;
            viewModel.OnOpenLogPage += OpenLogPage;
            var page = new VendingPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenKeyManagementPage(object sender, KeyManagementPageViewModel viewModel)
        {
            var page = new KeyManagementPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenUserManagementPage(object sender, UserManagementPageViewModel viewModel)
        {
            viewModel.OnOpenUserPage += OpenUserPage;
            var page = new UserManagementPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenLogPage(object sender, LogPageViewModel viewModel)
        {
            var page = new LogPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }
        private void OpenUserPage(object sender, UserPageViewModel viewModel)
        {
            var page = new UserPage(viewModel);
            NavigationPage.SetHasBackButton(page, true);
            MainPage.Navigation.PushAsync(page);
        }

        private MainPageViewModel mainViewModel = null;
	}
}
