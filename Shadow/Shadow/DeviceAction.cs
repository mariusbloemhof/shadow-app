using System;
using Xamarin;
using Newtonsoft.Json;
using Acr.UserDialogs;
using Plugin.BLE.Abstractions.Contracts;
using Shadow.Data;
using System.Threading.Tasks;

namespace Shadow
{
	public static class DeviceAction
	{
		private static ShadowDevice _Device;
		public static bool Pairing;
		public static string CharacteristicId;
		public static IProgressDialog ProgressDialog;

		public async static void WaitForDevicePair(string DeviceId)
		{
			CharacteristicId = "";

			if (Runtime.LastPairedDeviceID== DeviceId)
			{
				await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Information", "This device is already paired.", "OK");
				return;
			}

			foreach (ShadowDevice device in Shadow.Data.Runtime.DevicesFound)
			{
				if (device.Device.Id.ToString() == DeviceId)
				{
					_Device = device;
					break;
				}
			}

			Pairing = true;

			ProgressDialog = UserDialogs.Instance.Loading("Detecting Device. The device will vibrate once found. Press the panic button once you feel it vibrating to confirm.", null, "", true);

			Shadow.Data.Runtime.BLEDevice.SendPairVibrate(_Device);
		}

		public static async void DeviceConnected()
		{
			await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Information", "Device Connected. Press the button on your device once.", "OK");
		}

		public static async void PairDevice(ICharacteristic PairChr)
		{
			Pairing = false;
			ProgressDialog.Hide();
            SavePairedDevices();
            //this crashes on Android but works fine on IOS....
            //await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Information", "Device Paired.", "OK");
            ProgressDialog = UserDialogs.Instance.Loading("Device Paired.", null, "", true);
            await Task.Delay(2000);
            ProgressDialog.Hide();
        }

        public static async void PairFailed()
        {
            Pairing = false;
            ProgressDialog.Hide();
            await Shadow.Data.Runtime.MainDisplayInstance.DisplayAlert("Information", "Device Pairing timeout. Please try again", "OK");
        }

        public static void UnpairDevice(string GuidStr)
		{
            Shadow.Data.Runtime.BLEDevice.RemoveConnectedDevice();
			SavePairedDevices();

			if (Shadow.Data.Runtime.MainDisplayInstance != null)
			{
				((MainPage)Shadow.Data.Runtime.MainDisplayInstance).UpdatedConnectedDevicesLabel();
			}
		}

		private static void SavePairedDevices()
		{
            var json = JsonConvert.SerializeObject(Shadow.Data.Runtime.LastPairedDeviceID);
            bool result = Shadow.IO.LocalStorage.WriteTextFile("root", Shadow.Data.Const.LASTPAIREDDEVICE, json).Result;
            foreach (ShadowDevice device in Shadow.Data.Runtime.DevicesFound)
            {
                device.PairedIcon = "";
                if (device.Device.Id.ToString() == Shadow.Data.Runtime.LastPairedDeviceID)
                {
                    device.PairedIcon = "check.png";
                }
            }
        }

	}
}
