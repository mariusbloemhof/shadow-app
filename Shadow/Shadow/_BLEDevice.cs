﻿using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using Shadow.Data;
using Shadow.IO;
using System.Runtime.InteropServices;

namespace Shadow
{
	public class BLEDevice
	{
		private IAdapter _bleAdapter;

		private IBluetoothLE _bluetooth;
        private IService service;
        private ICharacteristic characteristic;

        public BLEDevice()
		{
            _bluetooth = CrossBluetoothLE.Current;
            _bleAdapter = CrossBluetoothLE.Current.Adapter;
            _bleAdapter.ScanTimeout = 15000;
                        
            Runtime.ConnectedDevices = new ObservableCollection<ShadowDevice>();
			Runtime.DevicesFound = new ObservableCollection<ShadowDevice>();
            _bleAdapter.DeviceDiscovered += bleDiscovered;
            _bleAdapter.DeviceConnectionLost += bleConnectionLost;
            _bleAdapter.ScanTimeoutElapsed += bleScanTimeout;
            _bleAdapter.DeviceConnected += bleDeviceConnected;
            
            if (Shadow.IO.LocalStorage.FileExists("root", Shadow.Data.Const.LASTPAIREDDEVICE).Result)
			{
				Shadow.Logger.LogDebug("AppDelegate", "FinishedLaunching 3", "");
                string json = LocalStorage.ReadTextFile("root", Shadow.Data.Const.LASTPAIREDDEVICE).Result;
                try
                {
                    Runtime.LastPairedDeviceID = JsonConvert.DeserializeObject<String>(json);
                    if (Runtime.LastPairedDeviceID != String.Empty)
                    {
                        _bleAdapter.ConnectToKnownDeviceAsync(new Guid(Runtime.LastPairedDeviceID));
                    }
                }
                catch
                { }
                    
            }
		}

		public void StartScanning()
		{
			_bleAdapter.StartScanningForDevicesAsync();
		}

		public void StopScanning()
		{
			_bleAdapter.StopScanningForDevicesAsync();
		}

        public void RemoveConnectedDevice()
        {
            foreach (IDevice device in _bleAdapter.ConnectedDevices)
            {
                if (device.State == Plugin.BLE.Abstractions.DeviceState.Connected)
                {
                    RemoveConnectedDevice(device);
                }
            }
        }

        private void RemoveConnectedDevice(IDevice device)
		{
            _bleAdapter.DisconnectDeviceAsync(device);
            Shadow.Data.ShadowDevice deviceToRemove = null;
			foreach (Shadow.Data.ShadowDevice connecteddevice in Shadow.Data.Runtime.ConnectedDevices)
			{
				if (connecteddevice.Device.Id == device.Id)
				{
					deviceToRemove = connecteddevice;
					break;
				}
			}
			if(deviceToRemove != null)
            {
                Shadow.Data.Runtime.ConnectedDevices.Remove(deviceToRemove);
                Shadow.Data.Runtime.LastPairedDeviceID = String.Empty;
                characteristic.ValueUpdated -= OnPanicPressed;
                service = null;
                characteristic = null;
            }
		}

		public async void bleConnectionLost(object sender, DeviceErrorEventArgs e)
		{
            RemoveConnectedDevice(e.Device);
			if(characteristic != null)
            	characteristic.ValueUpdated -= OnPanicPressed;
            service = null;
            characteristic = null;
			if (_bleAdapter.IsScanning)
				await _bleAdapter.StopScanningForDevicesAsync();
			_bleAdapter.ScanTimeout = 15000;
			await _bleAdapter.StartScanningForDevicesAsync();
		}
		public async void bleScanTimeout(object sender, EventArgs e)
		{
			/* ES Don't get this: if (currDevice != null)
			{
				_bleAdapter.ScanTimeoutElapsed -= bleScanTimeout;
			}
			else
			{
				_bleAdapter.ScanTimeout = 15000;
				await _bleAdapter.StartScanningForDevicesAsync();
			}*/
		}

        public async void bleDeviceConnected(object sender, DeviceEventArgs e)
        {
            try
            {
                IDevice device = e.Device;
                if ((device != null) &&
                    (device.Name == "SensorTag"))
                {
                    service = await device.GetServiceAsync(Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"));
                    characteristic = await service.GetCharacteristicAsync(Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"));
                    DeviceAction.CharacteristicId = characteristic.Uuid.ToString();
                    var bytes = await characteristic.ReadAsync();
                    characteristic.ValueUpdated += OnPanicPressed;
                    if (Shadow.Data.Runtime.DevicesFound.Count(c => c.Id == device.Id.ToString()) == 0)
                    {
                        Shadow.Data.Runtime.DevicesFound.Add(new ShadowDevice(device));
                    }
                    await characteristic.StartUpdatesAsync();
                    if (Shadow.Data.Runtime.ConnectedDevices.Count(c => c.Id == device.Id.ToString()) == 0)
                    {
                        Shadow.Data.Runtime.ConnectedDevices.Add(new ShadowDevice(device));
                    }
                    Shadow.Data.Runtime.LastPairedDeviceID = device.Id.ToString();
                    if (DeviceAction.Pairing)
                    {
                        await Vibrate(500, characteristic);
                        Device.StartTimer(new TimeSpan(0, 0, 0, 0, 10000), () =>
                        {
                            if (DeviceAction.Pairing)
                            {
                                _bleAdapter.DisconnectDeviceAsync(device);
								if(characteristic != null)
                                	characteristic.ValueUpdated -= OnPanicPressed;
                                characteristic = null;
                                service = null;
                                DeviceAction.PairFailed();
                                Shadow.Data.Runtime.LastPairedDeviceID = String.Empty;
                            }
                            return false;
                        });
                    }
                }
            }
            catch (DeviceConnectionException ex)
            {
                throw ex;
            }
        }

		public async void bleDiscovered(object sender, DeviceEventArgs e)
		{
			if (e.Device.Name == "SensorTag")
			{
				//if (_bleAdapter.IsScanning)
				//	await _bleAdapter.StopScanningForDevicesAsync();
                if (Shadow.Data.Runtime.DevicesFound.Count(c => c.Id == e.Device.Id.ToString()) == 0)
				{
					Shadow.Data.Runtime.DevicesFound.Add(new ShadowDevice(e.Device));
				}
			}
		}

        public async Task Vibrate(int DurationInMilSec, int DelayAfterMilSec = 0)
        {
            if (characteristic != null)
            {
                await characteristic.WriteAsync(new byte[] { 0x21 });
                await Task.Delay(DurationInMilSec);
                await characteristic.WriteAsync(new byte[] { 0x13 });
                if (DelayAfterMilSec != 0)
                {
                    await Task.Delay(DelayAfterMilSec);
                }
            }
        }

        public async Task Vibrate(int DurationInMilSec, ICharacteristic c, int DelayAfterMilSec = 0)
        {
            await c.WriteAsync(new byte[] { 0x21 });
            await Task.Delay(DurationInMilSec);
            await c.WriteAsync(new byte[] { 0x13 });
            if (DelayAfterMilSec != 0)
            {
                await Task.Delay(DelayAfterMilSec);
            }
        }

        public async void SendPairVibrate(ShadowDevice shadow)
		{
            Shadow.Data.Runtime.LastPairedDeviceID = String.Empty;
            if (shadow.Device.State == Plugin.BLE.Abstractions.DeviceState.Disconnected)
			{
                IDevice device = await _bleAdapter.ConnectToKnownDeviceAsync(shadow.Device.Id);
            }
        }

		public async void OnPanicPressed(object sender, CharacteristicUpdatedEventArgs e)
		{
			if (DeviceAction.Pairing)
			{
                await Vibrate(500, e.Characteristic);
                DeviceAction.PairDevice(e.Characteristic); // Vibrate only once paired
			}
			else
			{
                await Vibrate(100, e.Characteristic, 200);
                await Vibrate(500, e.Characteristic, 200);
                await Vibrate(100, e.Characteristic);
				App.SendAlert();
            }
		}
	}
}
