using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using Android.Content;
using System.Linq;
using HRVTestator.Gui;

namespace HRVTestator
{
    public class PolarH7Device : IDisposable
    {
        private const string DEVICE_NAME = "Polar";
        private const string CHARACTERISTIC_NAME = "Heart Rate Measurement";

        private Adapter adapter;
        private bool isDeviceContected = false;
        private IService heartRateService;
        private MainActivity mainActivity;

        public PolarH7Device(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public void Start() 
        {   
            adapter = new Adapter();
            adapter.DeviceDiscovered -= Adapter_DeviceDiscovered;
            adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
            adapter.StartScanningForDevices(Guid.Empty);
        }

        private void Adapter_DeviceDisconnected(object sender, DeviceConnectionEventArgs e)
        {
            string err = e.ErrorMessage;
            if (!string.IsNullOrEmpty(err))
            {
                throw new InvalidProgramException(err);
            }
        }

        private void Adapter_DeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
        {
            if (adapter.ConnectedDevices.Count > 0  || isDeviceContected == true)
            {
                return;
            }

            isDeviceContected = true;

            if (e.Device.Name == null || !e.Device.Name.Contains(DEVICE_NAME))
            {
                return;
            }

            if (adapter.IsScanning)
            {
                adapter.StopScanningForDevices();
            }

            adapter.DeviceConnected -= Adapter_DeviceConnected;
            adapter.DeviceConnected += Adapter_DeviceConnected;
            adapter.ConnectToDevice(e.Device);
        }

        private void Adapter_DeviceConnected(object sender, DeviceConnectionEventArgs e)
        {
            if (e.Device.Services.Count == 0)
            {
                e.Device.ServicesDiscovered -= Device_ServicesDiscovered;
                e.Device.ServicesDiscovered += Device_ServicesDiscovered;

                e.Device.DiscoverServices();
            }
            else
            {
                IService service = e.Device.Services.First(x => x.Name == "Heart Rate");
                ICharacteristic characteristic = service.Characteristics.First(x => x.Name.Contains(CHARACTERISTIC_NAME));
            }
        }

        private void Device_ServicesDiscovered(object sender, EventArgs e)
        {
            foreach (var device in adapter.ConnectedDevices)
            {
                if (device.Services.Count == 0)
                {
                    continue;
                }

                heartRateService = device.Services.FirstOrDefault(x => x.Name == "Heart Rate");
                heartRateService.CharacteristicsDiscovered -= HeartRateService_CharacteristicsDiscovered;
                heartRateService.CharacteristicsDiscovered += HeartRateService_CharacteristicsDiscovered;
                heartRateService.DiscoverCharacteristics();
            }
        }

        private void HeartRateService_CharacteristicsDiscovered(object sender, EventArgs e)
        {
            if (heartRateService.Characteristics.Count == 0)
            {
                return;
            }

            var characteristics = heartRateService.Characteristics;

            foreach (ICharacteristic characteristic in heartRateService.Characteristics)
            {
                if (!characteristic.Name.Contains(CHARACTERISTIC_NAME))
                {
                    return;
                }

                if (characteristic.CanUpdate)
                {
                    characteristic.ValueUpdated +=
                        (object s, CharacteristicReadEventArgs eventArgs) =>
                        {
                            byte[] data = eventArgs.Characteristic.Value;
                            var charactersticHandler = new Characteristic();
                            int[] rrValues = charactersticHandler.ExtractRRInterval(data);
                                var intent = new Intent("HEARD_RATE_UPDATE");
                            intent.PutExtra("HEARD_RATE", rrValues);

                            if (rrValues != null)
                            {
                                mainActivity.SendBroadcast(intent);
                            }

                        };

                    characteristic.StartUpdates();
                }
            }
        }

        public void Dispose()
        {
            if (adapter != null)
            {
                if (adapter.IsScanning)
                {
                    adapter.StopScanningForDevices();
                }

                if (adapter.ConnectedDevices.Count > 0)
                {
                    foreach (var device in adapter.ConnectedDevices)
                    {
                        if (device.Name != null && device.Name.Contains(DEVICE_NAME))
                        {
                            adapter.DeviceDisconnected -= Adapter_DeviceDisconnected;
                            adapter.DeviceDisconnected += Adapter_DeviceDisconnected;
                            adapter.DisconnectDevice(device);
                        }
                    }
                    if (adapter.ConnectedDevices.Count > 0)
                    {
                        adapter.ConnectedDevices[0] = null;
                    }
                    if (adapter.ConnectedDevices.Count == 2)
                    {
                        adapter.ConnectedDevices[1] = null;
                    }
                }

                adapter.Dispose();
                adapter = null;
            }
        }
    }
}