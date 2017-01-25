using System;
using Android.App;
using Android.Util;
using System.Threading;
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
using Android.Telephony;


namespace Shadow
{
	[Service]
	public class MyService : Android.App.Service
	{
		System.Threading.Timer _timer;

		public override void OnDestroy () {
			try
			{
				base.OnDestroy ();
				_timer.Dispose ();
				Log.Debug ("MyTestWorker", "MyTestWorker stopped");       
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "OnDestroy", ex.Message);
			}
		}

        private void StartBLE()
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

            Shadow.Data.Runtime.BLEDevice = new BLEDevice();
        }

		public void DoStuff ()  
		{

			try
			{
			/*	Shadow.Logger.LogDebug ("MyService", "OnReceive", "");

				DependencyService.Register<BluetoothLE.Core.IAdapter, BluetoothLE.Droid.Adapter>();

				Shadow.Logger.LogDebug ("MyService", "FinishedLaunching", "");

				Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 1", "");
				Shadow.Data.Runtime.ConnectedDevices = new ObservableCollection<IDevice> ();
				Shadow.Data.Runtime.DevicesFound = new ObservableCollection<IDevice> ();
				Shadow.Data.Runtime.PairedDevices = new ObservableCollection<Shadow.Data.TagDevice> ();
				Shadow.Data.Runtime.Contacts = new ObservableCollection<Shadow.Data.Contact> ();

				Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 2", "");
				if (Shadow.IO.LocalStorage.FileExists ("root", Shadow.Data.Const.PAIREDDEVICES).Result) {
					Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 3", "");
					string json = Shadow.IO.LocalStorage.ReadTextFile ("root", Shadow.Data.Const.PAIREDDEVICES).Result;
					Shadow.Data.Runtime.PairedDevices = JsonConvert.DeserializeObject<ObservableCollection<Shadow.Data.TagDevice>> (json);
				}

				if (Shadow.IO.LocalStorage.FileExists ("root", Shadow.Data.Const.CONTACTS).Result) {
					Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 3", "");
					string json = Shadow.IO.LocalStorage.ReadTextFile ("root", Shadow.Data.Const.CONTACTS).Result;
					Shadow.Data.Runtime.Contacts = JsonConvert.DeserializeObject<ObservableCollection<Shadow.Data.Contact>> (json);
				}


				Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 4", "");
				App.BluetoothAdapter.DeviceDiscovered += DeviceDiscovered;
				App.BluetoothAdapter.DeviceConnected += DeviceConnected;
				App.BluetoothAdapter.DeviceDisconnected += DeviceDisconnected;
				App.BluetoothAdapter.DeviceFailedToConnect += DeviceFailedToConnect;

				if (Shadow.Data.Runtime.PairedDevices.Count > 0) {
					Shadow.Logger.LogDebug ("MyService", "FinishedLaunching 5", "");
					App.BluetoothAdapter.StartScanningForDevices ();
				}

				_timer = new System.Threading.Timer ((o) => {
					Shadow.Logger.LogDebug ("MyService", "Timer", "");	
					if (!App.BluetoothAdapter.IsScanning) {
						Shadow.Logger.LogDebug ("MyService", "Start Scanning", "");	
						App.BluetoothAdapter.StartScanningForDevices ();
					}
				}
					, null, 0, 10000);*/
			}
			catch(Exception ex) {
				Shadow.Logger.LogDebug ("MyService", "DoStuff", ex.Message);
			}
		}

		public override Android.OS.IBinder OnBind (Android.Content.Intent intent) {
			try
			{
				Shadow.Logger.LogDebug ("MyService", "OnBind", "");
				return null;
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "OnBind", ex.Message);
				return null;
			}

		}

		private bool DevicePaired(string Guid)
		{
			try
			{
				/*foreach (Shadow.Data.TagDevice tagDevice in Shadow.Data.Runtime.PairedDevices) {
					if (tagDevice.Id == Guid)
						return true;
				}*/
				return false;
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DevicePaired", ex.Message);
				return false;
			}
		}

		/*private void DeviceFailedToConnect(object sender, DeviceConnectionEventArgs e)
		{
			try
			{
				Shadow.Logger.LogDebug ("MyService", "DeviceFailedToConnect", e.ErrorMessage);
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DeviceFailedToConnect", ex.Message);
			}
		}

		private void DeviceDiscovered (object sender, DeviceDiscoveredEventArgs e)
		{
			try
			{
				if ((e.Device.Name != null) && (e.Device.Name.Contains ("SensorTag"))) {
					Shadow.Logger.LogDebug ("MyService", "DeviceDiscovered 1", "");
					if (!DeviceAlreadyInList (Shadow.Data.Runtime.DevicesFound, e.Device)) {
						Shadow.Logger.LogDebug ("MyService", "DeviceDiscovered 2", "");
						e.Device.BackgroundColor = UIConst.LightGreyStr;
						e.Device.PairedIcon = "0";
						e.Device.BatteryLevel = "0";


						foreach (Shadow.Data.TagDevice paireddevice in Shadow.Data.Runtime.PairedDevices)
						{
							if (e.Device.GuidStr == paireddevice.Id)
							{
								e.Device.PairedIcon = "1";
							}
						}

						Shadow.Data.Runtime.DevicesFound.Add (e.Device);

						if (Shadow.Data.Runtime.PairedDevices.Count > 0) {
							if (DevicePaired (e.Device.GuidStr)) {
								Shadow.Logger.LogDebug ("MyService", "DeviceDiscovered 3", "");
								if (!DeviceAlreadyInList (Shadow.Data.Runtime.ConnectedDevices, e.Device)) {
									Shadow.Logger.LogDebug ("MyService", "DeviceDiscovered 4", "");
									App.BluetoothAdapter.ConnectToDevice (e.Device);
									Shadow.Logger.LogDebug ("MyService", "DeviceDiscovered 5", "");
								}
							}
						} 
					}
				}
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DeviceDiscovered", ex.Message);
			}
		}

		private bool DeviceAlreadyInList(IList<IDevice> devices, IDevice checkdevice)
		{
			try
			{
				foreach(IDevice device in devices)
				{
					if (device.GuidStr == checkdevice.GuidStr)
						return true;
				}
				return false;
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DeviceAlreadyInList", ex.Message);
				return false;
			}
		}

		private void DeviceConnected (object sender, DeviceConnectionEventArgs e)
		{
			try
			{
				Shadow.Logger.LogDebug ("MyService", "DeviceConnected", "");
				Shadow.Data.Runtime.ConnectedDevices.Add (e.Device);


				e.Device.ServiceDiscovered += ServicesDiscovered;
				e.Device.DiscoverServices ();
				//Navigation.PushAsync(new DevicePage(e.Device));
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DeviceConnected", ex.Message);
			}
		}

		private void DeviceDisconnected(object sender, DeviceConnectionEventArgs e)
		{
			try
			{
				Shadow.Logger.LogDebug ("MyService", "DeviceDisconnected", "");

				Shadow.Data.Runtime.ConnectedDevices.Remove (e.Device);
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "DeviceDisconnected", ex.Message);
			}
		}

		private void ServicesDiscovered(object sender, ServiceDiscoveredEventArgs e)
		{
			try
			{

				if (e.Service.Id.ToString () == Shadow.Data.Gatt.BATTERY_SERVICE) {
					//	App.BluetoothAdapter.ConnectedDevices[0].Write(e.Service.Id,
					//Debug.WriteLine ("d");
				}

				if (e.Service.Id.ToString () == Shadow.Data.Gatt.TX_POWER) {

				};

				if (e.Service.Id.ToString () == Shadow.Data.Gatt.DEVICE_TRIGGER) {
					//Debug.WriteLine ("f");
				};

				if (e.Service.IsPrimary) {

					Console.WriteLine (e.Service.Uuid);
					e.Service.CharacteristicDiscovered += CharacteristicDiscovered;
					e.Service.DiscoverCharacteristics ();
				}
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "ServicesDiscovered", ex.Message);
			}
		}

		private void CharacteristicDiscovered(object sender, CharacteristicDiscoveredEventArgs e)
		{
			try
			{
				//Shadow.Logger.LogDebug ("MyService", "CharacteristicValueUpdated", "");

				if (e.Characteristic.Uuid == "0000ffe1-0000-1000-8000-00805f9b34fb") {

					e.Characteristic.ValueUpdated += CharacteristicValueUpdated;

					//Debug.WriteLine (e.Characteristic.Id);
					if (e.Characteristic.Id.ToString () == Shadow.Data.Gatt.BATTERY_LEVEL) {
						e.Characteristic.Read ();
					}

					if (e.Characteristic.Id.ToString () == Shadow.Data.Gatt.TX_POWER_LEVEL) {
						e.Characteristic.Read ();
					}

					if (e.Characteristic.Id.ToString() == Shadow.Data.Gatt.DEVICE_TRIGGER)
					{
						if (DeviceAction.Pairing)
						{
							DeviceAction.CharacteristicId = e.Characteristic.Uuid.ToString();
							DeviceAction.DeviceConnected();
						}
					}

					if (e.Characteristic.CanUpdate) {
						e.Characteristic.StartUpdates ();
					}
				}
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "CharacteristicDiscovered", ex.Message);
			}
		}

		private void CharacteristicValueUpdated(object sender, CharacteristicReadEventArgs e)
		{
			try
			{
				Shadow.Logger.LogDebug ("MyService", "CharacteristicValueUpdated", e.Characteristic.Id.ToString());

				if (e.Characteristic.Id.ToString () == Shadow.Data.Gatt.BATTERY_LEVEL) {
					if(e.Characteristic.Value != null)
						System.Diagnostics.Debug.WriteLine (e.Characteristic.Value[0].ToString());
				} else if (e.Characteristic.Id.ToString () == Shadow.Data.Gatt.TX_POWER_LEVEL) {
					if(e.Characteristic.Value != null)
						System.Diagnostics.Debug.WriteLine (e.Characteristic.Value[0].ToString());
				} else {


					if (e.Characteristic.Id.ToString () == Shadow.Data.Gatt.DEVICE_TRIGGER) {
						if (e.Characteristic.RawValue != null) {

							//e.Characteristic.StopUpdates(); // ES: If we don't call this we keep getting triggers

							if (DeviceAction.Pairing)
							{

								DeviceAction.PairDevice(e.Characteristic);
								//DeviceAction.SendVibrate(e.Characteristic);

								//e.Characteristic.StopUpdates();
							}
							else
							{
								Shadow.Logger.LogDebug("MyService", "Trigger", DateTime.Now.ToLongTimeString());

								DeviceAction.SendVibrate(e.Characteristic);
								App.SendAlert();

								e.Characteristic.StopUpdates();

								foreach (IDevice device in App.BluetoothAdapter.ConnectedDevices)
								{
									device.Disconnect();
								}
								Shadow.Data.Runtime.ConnectedDevices.Clear();
								Shadow.Data.Runtime.DevicesFound.Clear();
								App.BluetoothAdapter.StartScanningForDevices();
							}
						}
					}
				}
			}
			catch(Exception ex) {
				Shadow.Logger.LogError ("MyService", "CharacteristicValueUpdated", ex.Message);
			}
		}*/
	}
}

