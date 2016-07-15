using Robotics.Mobile.Core.Bluetooth.LE;
using System;
using Android.Content;
using System.Linq;

namespace HRVTestator
{
    public class PolarH7Service : IDisposable
    {
        private const string DEVICE_NAME = "Polar";
        private const string CHARACTERISTIC_NAME = "Heart Rate Measurement";

        private Adapter adapter;
        private bool isDeviceContected = false;
        private IService heartRateService;
        private MainActivity mainActivity;

        public PolarH7Service(MainActivity mainActivity)
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
                            int[] rrValues = ExtractRRInterval(data); //DecodeHeartRateCharacteristicValue(eventArgs.Characteristic);
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

        private static int[] ExtractRRInterval(byte[] data) //BluetoothGattCharacteristic characteristic)
        {
            BluetoothGattCharacteristic characteristic = new BluetoothGattCharacteristic();
            characteristic.SetValue(data);
            int flag = characteristic.GetIntValue(BluetoothGattCharacteristic.FORMAT_UINT8, 0);
            int format = -1;
            int energy = -1;
            int offset = 1; // This depends on hear rate value format and if there is energy data
            int rr_count = 0;

            if ((flag & 0x01) != 0)
            {
                format = BluetoothGattCharacteristic.FORMAT_UINT16;
                //Log.d(TAG, "Heart rate format UINT16.");
                Console.Out.WriteLine("Heart rate format UINT16.");
                offset = 3;
            }
            else
            {
                format = BluetoothGattCharacteristic.FORMAT_UINT8;
                Console.Out.WriteLine("Heart rate format UINT8.");
                //Log.d(TAG, "Heart rate format UINT8.");
                offset = 2;
            }
            if ((flag & 0x08) != 0)
            {
                // calories present
                energy = characteristic.GetIntValue(BluetoothGattCharacteristic.FORMAT_UINT16, offset);
                offset += 2;
                Console.Out.WriteLine("Received energy: {}" + energy);
                //Log.d(TAG, "Received energy: {}" + energy);
            }
            if ((flag & 0x16) != 0)
            {
                // RR stuff.
                //Log.d(TAG, "RR stuff found at offset: " + offset);
                Console.Out.WriteLine("RR stuff found at offset: " + offset);
                //Log.d(TAG, "RR length: " + (characteristic.GetValue()).Length);
                Console.Out.WriteLine("RR length: " + (characteristic.GetValue()).Length);
                rr_count = ((characteristic.GetValue()).Length - offset) / 2;
                //Log.d(TAG, "RR length: " + (characteristic.GetValue()).Length);
                Console.Out.WriteLine("RR length: " + (characteristic.GetValue()).Length);
                //Log.d(TAG, "rr_count: " + rr_count);
                Console.Out.WriteLine("rr_count: " + rr_count);
                if (rr_count > 0)
                {
                    int[] mRr_values = new int[rr_count];
                    for (int i = 0; i < rr_count; i++)
                    {
                        mRr_values[i] = characteristic.GetIntValue(
                                BluetoothGattCharacteristic.FORMAT_UINT16, offset);
                        offset += 2;
                        //Log.d(TAG, "Received RR: " + mRr_values[i]);
                        Console.Out.WriteLine("Received RR: " + mRr_values[i]);
                    }
                    return mRr_values;
                }
            }
            //Log.d(TAG, "No RR data on this update: ");
            Console.Out.WriteLine("No RR data on this update: ");
            return null;
        }

        //private int DecodeHeartRateCharacteristicValue(ICharacteristic c)
        //{
        //    byte[] data = c.Value;
        //    if (c.ID != 0x2A37.UuidFromPartial())
        //    {
        //        throw new InvalidOperationException("Wrong Charactristic guid: " + c.ID);
        //    }

        //    ushort bpm = 0;
        //    if ((data[0] & 0x01) == 0)
        //    {
        //        bpm = data[1];
        //    }
        //    else
        //    {
        //        bpm = (ushort)data[1];
        //        bpm = (ushort)(((bpm >> 8) & 0xFF) | ((bpm << 8) & 0xFF00));
        //    }
        //    return bpm;
        //}
    }
}