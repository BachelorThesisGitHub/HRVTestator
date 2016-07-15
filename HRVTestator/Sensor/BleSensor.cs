using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HRVTestator.Sensor
{
    public abstract class BleSensor<T>
    {
        private static String TAG = typeof(BleSensor<T>).Name; // BleSensor.class.getSimpleName();
        private static String CHARACTERISTIC_CONFIG = "00002902-0000-1000-8000-00805f9b34fb";
        private T data;

        protected BleSensor()
        {
        }

        public abstract String GetName();

        public String GetCharacteristicName(String uuid)
        {
            if (GetDataUUID().Equals(uuid))
                return GetName() + " Data";
            else if (GetConfigUUID().Equals(uuid))
                return GetName() + " Config";
            return "Unknown";
        }

        public abstract String GetServiceUUID();
        public abstract String GetDataUUID();
        public abstract String GetConfigUUID();

        public bool IsConfigUUID(String uuid)
        {
            return false;
        }

        public T GetData()
        {
            return data;
        }

        public abstract String GetDataString();

        public void OnCharacteristicChanged(BluetoothGattCharacteristic c)
        {
            data = Parse(c);
        }

        public bool OnCharacteristicRead(BluetoothGattCharacteristic c)
        {
            return false;
        }

        protected byte[] GetConfigValues(bool enable)
        {
            return new byte[] { (byte)(enable ? 1 : 0) };
        }

        protected abstract T Parse(BluetoothGattCharacteristic c);

        public BluetoothGattExecutor.ServiceAction[] enable(readonly bool enable)
        {
            return new BluetoothGattExecutor.ServiceAction[] 
            {
                write(getConfigUUID(), getConfigValues(enable)),
                notify(enable)
        };
        }

        public BluetoothGattExecutor.ServiceAction Update()
        {
            return BluetoothGattExecutor.ServiceAction.NULL;
        }

        public BluetoothGattExecutor.ServiceAction Read(readonly  String uuid)
        {
            return new BluetoothGattExecutor.ServiceAction() {
            @Override
            public bool execute(BluetoothGatt bluetoothGatt)
        {
            final BluetoothGattCharacteristic characteristic = getCharacteristic(bluetoothGatt, uuid);
            bluetoothGatt.readCharacteristic(characteristic);
            return false;
        }
    };
}

public BluetoothGattExecutor.ServiceAction Write(readonly  String uuid, readonly  byte[] value)
{
    return new BluetoothGattExecutor.ServiceAction() {
            @Override
            public bool execute(BluetoothGatt bluetoothGatt)
{
    readonly BluetoothGattCharacteristic characteristic = getCharacteristic(bluetoothGatt, uuid);

    if (characteristic != null)
    {
        characteristic.setValue(value);
        bluetoothGatt.writeCharacteristic(characteristic);
        return false;
    }

    Log.i(TAG, "Characteristc not found with uuid: " + uuid);
    return true;
}
        };
    }

    public BluetoothGattExecutor.ServiceAction Notify(readonly bool start)
{
    return new BluetoothGattExecutor.ServiceAction() {
            @Override
            public bool execute(BluetoothGatt bluetoothGatt)
{
    readonly UUID CCC = UUID.fromString(CHARACTERISTIC_CONFIG);

    readonly BluetoothGattCharacteristic dataCharacteristic = getCharacteristic(bluetoothGatt, getDataUUID());
    readonly BluetoothGattDescriptor config = dataCharacteristic.getDescriptor(CCC);
    if (config == null)
        return true;

    // enable/disable locally
    bluetoothGatt.setCharacteristicNotification(dataCharacteristic, start);
    // enable/disable remotely
    config.setValue(start ? BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE : BluetoothGattDescriptor.DISABLE_NOTIFICATION_VALUE);
    bluetoothGatt.writeDescriptor(config);
    return false;
}
        };
    }

    private BluetoothGattCharacteristic GetCharacteristic(BluetoothGatt bluetoothGatt, String uuid)
{
    readonly UUID serviceUuid = UUID.fromString(GetServiceUUID());
    readonly UUID characteristicUuid = UUID.fromString(uuid);

    readonly BluetoothGattService service = bluetoothGatt.getService(serviceUuid);
    return service.getCharacteristic(characteristicUuid);
}
    }
}