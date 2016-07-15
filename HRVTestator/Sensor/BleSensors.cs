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
    public class BleSensors
    {
        private static HashMap<String, BleSensor<?>> SENSORS = new HashMap<String, BleSensor<?>>();

        static {
        final BleTestSensor testSensor = new BleTestSensor();
        final BleHeartRateSensor heartRateSensor = new BleHeartRateSensor();

        SENSORS.put(testSensor.getServiceUUID(), testSensor);
        SENSORS.put(heartRateSensor.getServiceUUID(), heartRateSensor);
    }

    public static BleSensor<?> getSensor(String uuid)
    {
        return SENSORS.get(uuid);
    }
}
}