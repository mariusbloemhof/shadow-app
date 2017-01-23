using System;
using Shadow;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Telephony;
using Shadow.Droid;

[assembly: Dependency(typeof(INativeFunctions_Android))]
namespace Shadow.Droid
{
	public class INativeFunctions_Android : INativeFunctions
	{
		public async void SendSMS(string Number, string Message)
		{
			SmsManager.Default.SendTextMessage(Number, null,Message, null, null);
		}
	}
}
