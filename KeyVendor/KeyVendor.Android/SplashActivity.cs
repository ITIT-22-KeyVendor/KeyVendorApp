using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace KeyVendor.Droid
{
    [Activity(Label = "KeyVendor",
              Theme = "@style/KeyVendor.SplashScreen",
              Icon = "@mipmap/icon",
              MainLauncher = true,
              NoHistory = true)]
    public class SplashActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }

        public override void OnBackPressed() { }
    }
}