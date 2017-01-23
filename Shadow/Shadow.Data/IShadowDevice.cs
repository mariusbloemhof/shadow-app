using System;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;

namespace Shadow.Data
{
	public class ShadowDevice
	{
		public IDevice Device { get; set; }
		public string PanicCharacteristicUUID { get; set; }
		public string Id { get; set; }
		public string Name { get; set; }
		public string PairedIcon { get; set; }

		public ShadowDevice(IDevice device)
		{
			this.Device = device;
			this.Id = device.Id.ToString();
			this.Name = device.Name;
		}

	}
}
