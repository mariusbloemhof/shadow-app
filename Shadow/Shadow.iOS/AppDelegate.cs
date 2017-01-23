using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using Shadow;


using Foundation;
using UIKit;
using CoreLocation;
using TK.CustomMap.iOSUnified;
using Xamarin.Forms;
/*using BluetoothLE.Core;
using BluetoothLE.iOS;
using BluetoothLE.Core.Events;*/
using Newtonsoft.Json;

namespace Shadow.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
		private const int SCAN_TIMEOUT = 3000;
		private static DateTime _lastAlert;
		private UILocalNotification _lastNotification;
		private object lockObject;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
			Xamarin.FormsMaps.Init();
			TKCustomMapRenderer.InitMapRenderer();
			NativePlacesApi.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
					UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
				);

				app.RegisterUserNotificationSettings(notificationSettings);
			}


			var locationManager = new CLLocationManager();
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				locationManager.RequestAlwaysAuthorization(); // works in background
				//locationManager.RequestWhenInUseAuthorization (); // only in foreground
			}

			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
			UIApplication.SharedApplication.SetStatusBarHidden(false, false);
			Shadow.UIHelper.ScreenSize = new Size(UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);

			/*DependencyService.Register<IAdapter, Adapter>();

			Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching", "");

			Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 1", "");
			Shadow.Data.Runtime.ConnectedDevices = new ObservableCollection<IDevice>();
			Shadow.Data.Runtime.DevicesFound = new ObservableCollection<IDevice>();
			Shadow.Data.Runtime.PairedDevices = new ObservableCollection<Shadow.Data.TagDevice>();


			Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 2", "");
			if (Shadow.IO.LocalStorage.FileExists("root", Shadow.Data.Const.PAIREDDEVICES).Result)
			{
				Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 3", "");
				string json = Shadow.IO.LocalStorage.ReadTextFile("root", Shadow.Data.Const.PAIREDDEVICES).Result;
				Shadow.Data.Runtime.PairedDevices = JsonConvert.DeserializeObject<ObservableCollection<Shadow.Data.TagDevice>>(json);
			}



			Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 4", "");
			App.BluetoothAdapter.DeviceDiscovered += DeviceDiscovered;
			App.BluetoothAdapter.DeviceConnected += DeviceConnected;
			App.BluetoothAdapter.DeviceDisconnected += DeviceDisconnected;
			App.BluetoothAdapter.DeviceFailedToConnect += DeviceFailedToConnect;

			//if (Shadow.Data.Runtime.PairedDevices.Count > 0)
			{
				Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 5", "");
				App.BluetoothAdapter.StartScanningForDevices();
			}
			*/
			lockObject = 1;
			var timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromSeconds(10), delegate
			{
				//DoLocalNotifications();
			});	


            LoadApplication(new Shadow.App());

            return base.FinishedLaunching(app, options);
            
        }

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			// show an alert
			UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			//Window.RootViewController.PresentViewController(okayAlertController, true, null);

			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		private void DoLocalNotifications()
		{
			lock(lockObject)
			{
				if (_lastNotification != null)
					UIApplication.SharedApplication.CancelLocalNotification(_lastNotification);

				_lastNotification = new UILocalNotification();

				// set the fire date (the date time in which it will fire)
				_lastNotification.FireDate = NSDate.FromTimeIntervalSinceNow(30);

				// configure the alert
				_lastNotification.AlertAction = "Shadow Notice";
				_lastNotification.AlertBody = "Please note: Shadow will not be able to detect the device signal if the app is not running. Please start the Shadow app.";

				// modify the badge
				_lastNotification.ApplicationIconBadgeNumber = 1;

				// set the sound to be the default sound
				_lastNotification.SoundName = UILocalNotification.DefaultSoundName;

				// schedule it
				UIApplication.SharedApplication.ScheduleLocalNotification(_lastNotification);
			}
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.

			Shadow.Logger.LogDebug("AppDelegate", "OnResignActivation", "");
			Console.WriteLine("App moving to inactive state.");
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
			Console.WriteLine("App entering background state.");
			Console.WriteLine("Now receiving location updates in the background");
		



			Shadow.Logger.LogDebug("AppDelegate", "DidEnterBackground", "");

			/*ES0407 if (!App.BluetoothAdapter.IsScanning) {
				App.BluetoothAdapter.StartScanningForDevices ();
			}*/

			/*
			if (!App.BluetoothAdapter.IsScanning) {
				ConnectedDevices = new ObservableCollection<IDevice> ();

				App.BluetoothAdapter.DeviceDiscovered += DeviceDiscovered;
				App.BluetoothAdapter.DeviceConnected += DeviceConnected;
				App.BluetoothAdapter.DeviceDisconnected += DeviceDisconnected;
				App.BluetoothAdapter.StartScanningForDevices ();
			}*/

		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
			Console.WriteLine("App will enter foreground");
			Shadow.Logger.LogDebug("AppDelegate", "WillEnterForeground", "");
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
			Console.WriteLine("App is becoming active");
			Shadow.Logger.LogDebug("AppDelegate", "OnActivated", "");
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
			Shadow.Logger.LogDebug("AppDelegate", "WillTerminate", "");
		}

		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			Shadow.Logger.LogDebug("AppDelegate", "RegisteredForRemoteNotifications", "");
		}

		/*static bool DevicePaired(string Guid)
		{
			foreach (Shadow.Data.TagDevice tagDevice in Shadow.Data.Runtime.PairedDevices)
			{
				if (tagDevice.Id == Guid)
					return true;
			}
			return false;
		}

		static void DeviceFailedToConnect(object sender, DeviceConnectionEventArgs e)
		{
			Shadow.Logger.LogDebug("AppDelegate", "DeviceFailedToConnect", e.ErrorMessage);

		}

		static void DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
		{
			if ((e.Device.Name != null) && (e.Device.Name.Contains("SensorTag")))
			{
				Shadow.Logger.LogDebug("AppDelegate", "DeviceDiscovered 1", "");
				if (!DeviceAlreadyInList(Shadow.Data.Runtime.DevicesFound, e.Device))
				{
					Shadow.Logger.LogDebug("AppDelegate", "DeviceDiscovered 2", "");
					e.Device.BackgroundColor = UIConst.LightGreyStr;
					e.Device.PairedIcon = "0";
					e.Device.BatteryLevel = "0";

					foreach (Shadow.Data.TagDevice paireddevice in Shadow.Data.Runtime.PairedDevices)
					{
						if (e.Device.GuidStr == paireddevice.Id)
						{
							e.Device.PairedIcon = "check.png";
						}
					}

					Shadow.Data.Runtime.DevicesFound.Add(e.Device);

					if (Shadow.Data.Runtime.PairedDevices.Count > 0)
					{
						if (DevicePaired(e.Device.GuidStr))
						{
							Shadow.Logger.LogDebug("AppDelegate", "DeviceDiscovered 3", "");
							if (!DeviceAlreadyInList(Shadow.Data.Runtime.ConnectedDevices, e.Device))
							{
								Shadow.Logger.LogDebug("AppDelegate", "DeviceDiscovered 4", "");
								App.BluetoothAdapter.ConnectToDevice(e.Device);
								Shadow.Logger.LogDebug("AppDelegate", "DeviceDiscovered 5", "");
							}
						}
					}
				}
			}
		}

		static bool DeviceAlreadyInList(IList<IDevice> devices, IDevice checkdevice)
		{
			foreach (IDevice device in devices)
			{
				if (device == checkdevice)
					return true;
			}
			return false;
		}

		static void DeviceConnected(object sender, DeviceConnectionEventArgs e)
		{
			Shadow.Logger.LogDebug("AppDelegate", "DeviceConnected", "");
			Shadow.Data.Runtime.ConnectedDevices.Add(e.Device);

			e.Device.ServiceDiscovered += ServicesDiscovered;
			e.Device.DiscoverServices();
			//Navigation.PushAsync(new DevicePage(e.Device));
		}

		static void DeviceDisconnected(object sender, DeviceConnectionEventArgs e)
		{
			Shadow.Logger.LogDebug("AppDelegate", "DeviceDisconnected", "");

			Shadow.Data.Runtime.ConnectedDevices.Remove(e.Device);
		}

		static void ServicesDiscovered(object sender, ServiceDiscoveredEventArgs e)
		{
			if (e.Service.Id.ToString() == Shadow.Data.Gatt.BATTERY_SERVICE)
			{
				//	App.BluetoothAdapter.ConnectedDevices[0].Write(e.Service.Id,
				Debug.WriteLine("d");
			}

			if (e.Service.Id.ToString() == Shadow.Data.Gatt.TX_POWER)
			{

			};

			if (e.Service.Id.ToString() == Shadow.Data.Gatt.DEVICE_TRIGGER)
			{
				Debug.WriteLine("f");
			};

			e.Service.CharacteristicDiscovered += CharacteristicDiscovered;
			e.Service.DiscoverCharacteristics();
		}

		static void CharacteristicDiscovered(object sender, CharacteristicDiscoveredEventArgs e)
		{
			//Shadow.Logger.LogDebug ("AppDelegate", "CharacteristicValueUpdated", "");

			e.Characteristic.ValueUpdated += CharacteristicValueUpdated;

			if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.TX_POWER_LEVEL)
			{
				e.Characteristic.Read();
			}


			if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.DEVICE_TRIGGER)
			{
				if (DeviceAction.Pairing)
				{
					DeviceAction.CharacteristicId = e.Characteristic.Uuid.ToString();
					DeviceAction.DeviceConnected();
				}
			}

			if (e.Characteristic.CanUpdate)
			{
				e.Characteristic.StartUpdates();	
			}
		}

		static void CharacteristicValueUpdated(object sender, CharacteristicReadEventArgs e)
		{
			//Shadow.Logger.LogDebug ("AppDelegate", "CharacteristicValueUpdated", "");

			if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.BATTERY_LEVEL)
			{
				if(e.Characteristic.RawValue != null)
					Debug.WriteLine (e.Characteristic.RawValue);
			}
			else if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.TX_POWER_LEVEL)
			{
				//Debug.WriteLine (e.Characteristic.Value);
			}
			else 
			{
				if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.DEVICE_TRIGGER)
				{
					if (e.Characteristic.RawValue != null)
					{
						if (DeviceAction.Pairing)
						{

							DeviceAction.PairDevice(e.Characteristic);

							// ES: If we don't call this we keep getting triggers
							//e.Characteristic.StopUpdates();
						}
						else
						{
							Shadow.Logger.LogDebug("AppDelegate", "Trigger", DateTime.Now.ToLongTimeString());

							DeviceAction.SendVibrate(e.Characteristic);
							App.SendAlert();

							// ES: If we don't call this we keep getting triggers

							e.Characteristic.StopUpdates();

							foreach (IDevice device in App.BluetoothAdapter.ConnectedDevices)
							{
								device.Disconnect();
							}
							Shadow.Data.Runtime.ConnectedDevices.Clear();
							Shadow.Data.Runtime.DevicesFound.Clear();
							App.BluetoothAdapter.StartScanningForDevices();

							//e.Characteristic.StartUpdates();



						}
					}
				}
			}
		}*/
    }
}
