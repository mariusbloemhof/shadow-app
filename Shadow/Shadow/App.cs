using System;
using Xamarin.Forms;
//using BluetoothLE.Core;
//using BluetoothLE.Core.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Plugin.Messaging;
using Shadow.Model;

namespace Shadow
{
	public class App : Application
	{
		/*private static readonly IAdapter _bluetoothAdapter;
		public static IAdapter BluetoothAdapter { get { return _bluetoothAdapter; } }*/

		static App()
		{
            Shadow.Data.Runtime.Contacts = new ObservableCollection<Shadow.Data.Contact>();
            if (Shadow.IO.LocalStorage.FileExists("root", Shadow.Data.Const.CONTACTS).Result)
            {
                Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 3", "");
                string json = Shadow.IO.LocalStorage.ReadTextFile("root", Shadow.Data.Const.CONTACTS).Result;
                Shadow.Data.Runtime.Contacts = JsonConvert.DeserializeObject<ObservableCollection<Shadow.Data.Contact>>(json);
            }

            Shadow.Data.Runtime.MessageLine1 = Shadow.Lang.AppResources.DefaultMessage1;
			Shadow.Data.Runtime.MessageLine2 = Shadow.Lang.AppResources.DefaultMessage2;

			Xamarin.Forms.DependencyService.Register<INativeFunctions>();
            Shadow.Data.Runtime.BLEDevice = new BLEDevice();
        }

		public App()
		{
			//MainPage = new ScanDevicesPage ();
			//MainPage = new MainPage();
			//MainPage = new WelcomePage();
			//MainPage = new EmergencyMessagePage();
			//MainPage = new SignupDevice();
			//MainPage = new MainPage();
			//MainPage = new SignupPageWizard();
			//MainPage = new ContactPickerPage();
		}

		protected async override void OnStart()
		{
            //ShadowServiceHandle when your app starts
			MainPage = new WelcomePage();
            return;

            Account account = await ShadowService.GetLoggedinUser();
			if((account == null) || (string.IsNullOrEmpty(account.firstName)))
				MainPage = new WelcomePage();
			else
				MainPage = new MainPage();
		}

		protected async override void OnResume()
		{
            // Handle when your app resumes
           /* MainPage = new MainPage();
            return;*/
            Account account = await ShadowService.GetLoggedinUser();
			if ((account == null) || (string.IsNullOrEmpty(account.firstName)))
				MainPage = new WelcomePage();
			else
				MainPage = new MainPage();
		}

		public static async void SendAlert()
		{
			try
			{
				var locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;
				string url = "";

				var position = await locator.GetPositionAsync(10000);
				url = "http://maps.google.com/maps?q=" + position.Latitude.ToString().Replace(",", ".") + "," + position.Longitude.ToString().Replace(",", ".");

				string msg = Shadow.Data.Runtime.MessageLine1 + "." + Shadow.Data.Runtime.MessageLine2 + ". " + url;

				if (Shadow.Data.Runtime.TestMode)
				{
					await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Alert", msg, "OK");
				}
				else
				{
					foreach (Shadow.Data.Contact contact in Shadow.Data.Runtime.Contacts)
					{
						if (contact.Mobile != "")
						{
							string mobile = contact.Mobile;

							mobile = mobile.Replace("(", "").Replace(")", "").Replace(" ", "");
							if (mobile.StartsWith("0"))
								mobile = "27" + mobile.Remove(0, 1);
							mobile = mobile.Replace("\u00a0", "");
							//mobile = "27836564309";
							if (Device.OS == TargetPlatform.iOS)
							{
								//Shadow.Services.HTTP http = new Shadow.Services.HTTP();
								//string res = http.GetRequest("http://bulksms.2way.co.za/eapi/submission/send_sms/2/2.0?username=erhard&password=verbatim1&message=" + System.Web.HttpUtility.UrlEncode(msg) + "&msisdn=" + mobile);
								bool result = await ShadowService.sendSMS(mobile, msg);
								Shadow.Logger.LogDebug("AppDelegate", "Trigger SMS Result", result.ToString());
							}
							else
							{
								var smsMessenger = MessagingPlugin.SmsMessenger;
								if (smsMessenger.CanSendSms)
								{
									smsMessenger.SendSms(mobile, msg);
								}
								//DependencyService.Get<INativeFunctions>().SendSMS(mobile, msg);
							}
						}
					}

				}
			}
			catch (Exception ex)
			{
				Shadow.Logger.LogError("App", "SendAlert", ex.Message);
			}
		}
	}
}
