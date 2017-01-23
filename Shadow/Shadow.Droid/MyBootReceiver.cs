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


namespace Shadow.Droid
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { Android.Content.Intent.ActionBootCompleted })]
	public class MyBootReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			try
			{
				if ((intent.Action != null) && (intent.Action == Intent.ActionBootCompleted)) { 

					Intent startIntent = new Intent(context, typeof(MyService));
					context.StartService(startIntent);

				} else {
					Shadow.Logger.LogDebug ("MyBootReceiver", "OnReceive", "Could not start on ActionBootCompleted.");
				}
			}
			catch (Exception ex)
			{
				Shadow.Logger.LogError("MyBootReceiver", "OnReceive", ex.Message);
			}
		}



	}
}

