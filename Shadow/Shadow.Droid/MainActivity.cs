using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
/*using BluetoothLE.Core;
using BluetoothLE.Droid;
using BluetoothLE.Core.Events;*/
using Newtonsoft.Json;
using Acr.UserDialogs;

namespace Shadow.Droid
{
    [Activity(Label = "Shadow", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

			//ES22 DependencyService.Register<BluetoothLE.Core.IAdapter, BluetoothLE.Droid.Adapter>();

			Shadow.UIHelper.ScreenSize = new Xamarin.Forms.Size((int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density), (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density));

			Xamarin.FormsMaps.Init(this, bundle);

			UserDialogs.Init(() => (Activity)this);

			StartService(new Intent(this, typeof(MyService)));

            LoadApplication(new Shadow.App());

           
        }

      
    }
}

