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
    public class BleSensorUtils
    {
        private BleSensorUtils()
        {
        }

        /**
         * Gyroscope, Magnetometer, Barometer, IR temperature
         * all store 16 bit two's complement values in the awkward format
         * LSB MSB, which cannot be directly parsed as getIntValue(FORMAT_SINT16, offset)
         * because the bytes are stored in the "wrong" direction.
         *
         * This function extracts these 16 bit two's complement values.
         * */
        public static Integer shortSignedAtOffset(BluetoothGattCharacteristic c, int offset)
        {
            Integer lowerByte = c.getIntValue(FORMAT_UINT8, offset);
            if (lowerByte == null)
                return 0;
            Integer upperByte = c.getIntValue(FORMAT_SINT8, offset + 1); // Note: interpret MSB as signed.
            if (upperByte == null)
                return 0;

            return (upperByte << 8) + lowerByte;
        }

        public static Integer shortUnsignedAtOffset(BluetoothGattCharacteristic c, int offset)
        {
            Integer lowerByte = c.getIntValue(FORMAT_UINT8, offset);
            if (lowerByte == null)
                return 0;
            Integer upperByte = c.getIntValue(FORMAT_UINT8, offset + 1); // Note: interpret MSB as unsigned.
            if (upperByte == null)
                return 0;

            return (upperByte << 8) + lowerByte;
        }
    }
}