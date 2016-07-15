using Android.App;
using Android.Content;
using HRVTestator.Gui;
using System;
using System.Collections.Generic;

namespace HRVTestator
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "HEARD_RATE_UPDATE" })]
    public class HeartRateReceiver : BroadcastReceiver
    {      
        private static MainActivity mainActivity;
        private static HRV hrv;

        public HeartRateReceiver(){ } //Strange but Android call also once the default constructer, dont know why:

        public HeartRateReceiver(MainActivity mainActivity, HRV hrv)
        {
            HeartRateReceiver.mainActivity = mainActivity;
            HeartRateReceiver.hrv = hrv;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                int[] rrValues = intent.GetIntArrayExtra("HEARD_RATE");

                IEnumerable<Measurement> validMeasurements = HeartRateReceiver.hrv.SetRR(rrValues);

                foreach (Measurement measurement in validMeasurements)
                {
                    // Do not print the first hrv-Values
                    if (measurement.HRV != 0)
                    {
                        HeartRateReceiver.mainActivity.UpdateHeartRateView(measurement.HRV);
                    }
                }
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("Fehler beim empfangen der Daten:.....e.InnerException");
            }
        }
    }
}