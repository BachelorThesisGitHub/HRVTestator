using System;

namespace HRVTestator
{
    public class BluetoothGattCharacteristic
    {
        /**
         * Characteristic value format type uint8
         */
        public const int FORMAT_UINT8 = 0x11;

        /**
         * Characteristic value format type uint16
         */
        public const int FORMAT_UINT16 = 0x12;

        /**
         * Characteristic value format type uint32
         */
        public const int FORMAT_UINT32 = 0x14;

        /**
         * Characteristic value format type sint8
         */
        public const int FORMAT_SINT8 = 0x21;

        /**
         * Characteristic value format type sint16
         */
        public const int FORMAT_SINT16 = 0x22;

        /**
         * Characteristic value format type sint32
         */
        public const int FORMAT_SINT32 = 0x24;

        /**
         * The cached value of this characteristic.
         * @hide
         */
        protected byte[] mValue;


        /**
         * Get the stored value for this characteristic.
         *
         * <p>This function returns the stored value for this characteristic as
         * retrieved by calling {@link BluetoothGatt#readCharacteristic}. The cached
         * value of the characteristic is updated as a result of a read characteristic
         * operation or if a characteristic update notification has been received.
         *
         * @return Cached value of the characteristic
         */
        public byte[] GetValue()
        {
            return mValue;
        }

        /**
         * Return the stored value of this characteristic.
         *
         * <p>The formatType parameter determines how the characteristic value
         * is to be interpreted. For example, settting formatType to
         * {@link #FORMAT_UINT16} specifies that the first two bytes of the
         * characteristic value at the given offset are interpreted to generate the
         * return value.
         *
         * @param formatType The format type used to interpret the characteristic
         *                   value.
         * @param offset Offset at which the integer value can be found.
         * @return Cached value of the characteristic or null of offset exceeds
         *         value size.
         */
        public int GetIntValue(int formatType, int offset)
        {
            if ((offset + GetTypeLen(formatType)) > mValue.Length) return 999; //vorher: Null

            switch (formatType)
            {
                case FORMAT_UINT8:
                    return UnsignedByteToInt(mValue[offset]);

                case FORMAT_UINT16:
                    return UnsignedBytesToInt(mValue[offset], mValue[offset + 1]);

                case FORMAT_UINT32:
                    return UnsignedBytesToInt(mValue[offset], mValue[offset + 1],
                                              mValue[offset + 2], mValue[offset + 3]);
                case FORMAT_SINT8:
                    return UnsignedToSigned(UnsignedByteToInt(mValue[offset]), 8);

                case FORMAT_SINT16:
                    return UnsignedToSigned(UnsignedBytesToInt(mValue[offset],
                                                               mValue[offset + 1]), 16);

                case FORMAT_SINT32:
                    return UnsignedToSigned(UnsignedBytesToInt(mValue[offset],
                            mValue[offset + 1], mValue[offset + 2], mValue[offset + 3]), 32);
            }

            return 999; //vorher: Null
        }

        public bool SetValue(byte[] value)
        {
            mValue = value;
            return true;
        }


        /**
         * Returns the size of a give value type.
         */
        private int GetTypeLen(int formatType)
        {
            return formatType & 0xF;
        }

        /**
         * Convert a signed byte to an unsigned int.
         */
        private int UnsignedByteToInt(byte b)
        {
            return b & 0xFF;
        }

        /**
         * Convert signed bytes to a 16-bit unsigned int.
         */
        private int UnsignedBytesToInt(byte b0, byte b1)
        {
            return (UnsignedByteToInt(b0) + (UnsignedByteToInt(b1) << 8));
        }

        /**
         * Convert signed bytes to a 32-bit unsigned int.
         */
        private int UnsignedBytesToInt(byte b0, byte b1, byte b2, byte b3)
        {
            return (UnsignedByteToInt(b0) + (UnsignedByteToInt(b1) << 8))
                 + (UnsignedByteToInt(b2) << 16) + (UnsignedByteToInt(b3) << 24);
        }

        /**
         * Convert signed bytes to a 16-bit short float value.
         */
        private float bytesToFloat(byte b0, byte b1)
        {
            int mantissa = UnsignedToSigned(UnsignedByteToInt(b0)
                            + ((UnsignedByteToInt(b1) & 0x0F) << 8), 12);
            int exponent = UnsignedToSigned(UnsignedByteToInt(b1) >> 4, 4);
            return (float)(mantissa * Math.Pow(10, exponent));
        }



        /**
         * Convert an unsigned integer value to a two's-complement encoded
         * signed value.
         */
        private int UnsignedToSigned(int unsigned, int size)
        {
            if ((unsigned & (1 << size - 1)) != 0)
            {
                unsigned = -1 * ((1 << size - 1) - (unsigned & ((1 << size - 1) - 1)));
            }
            return unsigned;
        }
    }
}