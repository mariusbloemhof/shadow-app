using System;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Shadow.Data
{
	public static class Runtime
	{
		public static BLEDevice BLEDevice;
		public static string LastErrorMessage = "";
		public static ObservableCollection<ShadowDevice> DevicesFound { get; set; }
		public static ObservableCollection<ShadowDevice> ConnectedDevices { get; set; }
//		public static ObservableCollection<TagDevice> PairedDevices { get; set; }
        public static string LastPairedDeviceID { get; set; }
        public static ObservableCollection<Contact> Contacts { get; set; }
		public static string MessageLine1 { get; set; }
		public static string MessageLine2 { get; set; }
		public static RemLogin RemLogin { get; set; }
		public static ContentPage MainDisplayInstance { get; set; }
		public static bool TestMode { get; set; }
		public static bool TestModeOk { get; set; }
		public static void SaveRemLogin()
		{
			var json = JsonConvert.SerializeObject(Shadow.Data.Runtime.RemLogin);
			bool result = Shadow.IO.LocalStorage.WriteTextFile("root", Shadow.Data.Const.ACCOUNT, json).Result;
		}
	}
}

