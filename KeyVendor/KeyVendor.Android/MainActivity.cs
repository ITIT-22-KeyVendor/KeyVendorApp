using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace KeyVendor.Droid
{
    [Activity(Label = "KeyVendor", 
              Icon = "@mipmap/icon", 
              Theme = "@style/KeyVendor.SplashScreen",
              MainLauncher = true,
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
              LaunchMode = LaunchMode.SingleTop)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {        
        protected async override void OnCreate(Bundle bundle)
        {            
            base.Window.RequestFeature(WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.KeyVendor);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            Window.SetStatusBarColor(Android.Graphics.Color.DimGray);

            await TryToGetPermissions();
        }

        #region RuntimePermissions
        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
                return;
            }
        }
        async Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessCoarseLocation;

            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                return;
            }
            if (ShouldShowRequestPermissionRationale(permission))
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Потрібний дозвіл");
                alert.SetMessage("Застосунку необхідно отримати дозвіл на використання Bluetooth");
                alert.SetPositiveButton("Дозволити", (senderAlert, args) =>
                    { RequestPermissions(PermissionsGroup, RequestLocationId); });
                alert.SetNegativeButton("Скасувати", (senderAlert, args) =>
                    { Toast.MakeText(this, "Дозволи не отримано", ToastLength.Short).Show(); });

                Dialog dialog = alert.Create();
                dialog.Show();

                return;
            }

            RequestPermissions(PermissionsGroup, RequestLocationId);
        }

        public override async void OnRequestPermissionsResult(
            int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestLocationId:
                    {
                        if (grantResults.Length > 0 && grantResults[0] != (int)Permission.Granted)
                            Toast.MakeText(this, "Немає необхідних дозволів", ToastLength.Short).Show();

                    }
                    break;
            }
        }

        const int RequestLocationId = 0;
        readonly string[] PermissionsGroup =
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.AccessCoarseLocation
        };        
        #endregion
    }
}