using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using PinView.Droid;
using PinView.Abstractions;
using System.Threading.Tasks;

namespace Sample.Droid
{
    [Activity(Label = "Sample.Droid", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        private PinWidget _pinView;

        public void PinEntered(string pin)
        {
            Toast.MakeText(this, pin, ToastLength.Long).Show();
            _pinView.DigitCount = 4;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _pinView = FindViewById<PinWidget>(Resource.Id.MyButton);
            //_pinView.PinCompleted += PinComplete;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _pinView.RequestFocus();
        }

        private void PinComplete(object sender, PinCompletedEventArgs e)
        {
            Toast.MakeText(this, e.Pin, ToastLength.Long).Show();
            _pinView.DigitCount = 4;
        }
    }
}

