using System;

namespace Shadow.Data
{
	public static class Gatt
	{
		// Services
		public const string DEVICE_INFORMATION = "0000180a-0000-1000-8000-00805f9b34fb";
		public const string LINK_LOSS = "00001803-0000-1000-8000-00805f9b34fb";
		public const string IMMEDIATE_ALERT = "00001802-0000-1000-8000-00805f9b34fb";
		public const string TX_POWER = "00001804-0000-1000-8000-00805f9b34fb";
		public const string BATTERY_SERVICE = "0000180f-0000-1000-8000-00805f9b34fb";

		// Characteristics
		public const string SYSTEM_ID = "00002a23-0000-1000-8000-00805f9b34fb";
		public const string FIRMWARE_REVISION_STRING = "00002a26-0000-1000-8000-00805f9b34fb";
		public const string MANUFACTURER_NAME_STRING = "00002a29-0000-1000-8000-00805f9b34fb";
		public const string IEEE_CERTIFICATION = "00002a2a-0000-1000-8000-00805f9b34fb";
		public const string PNP_ID = "00002a50-0000-1000-8000-00805f9b34fb";
		public const string ALERT_LEVEL = "00002a06-0000-1000-8000-00805f9b34fb";
		public const string TX_POWER_LEVEL = "00002a07-0000-1000-8000-00805f9b34fb";
		public const string BATTERY_LEVEL = "00002a19-0000-1000-8000-00805f9b34fb";

		public const string DEVICE_TRIGGER = "0000ffe1-0000-1000-8000-00805f9b34fb";
	}
}

