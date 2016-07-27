using Android.App;
using Android.Content;
using HRVTestator.Gui;
using System;
using System.Collections.Generic;

namespace HRVTestator
{
    /// <summary>
    /// Die Klasse <see cref="HeartRateReceiver"/> ist verantwortlich für das Empfangen der Daten vom Sensor.
    /// Dies ist nötig da die Daten in der Klasse <see cref="PolarH7Device"/> nicht auf dem Main-Thread empfangen werden, 
    /// aber für die Darstellung den MainThread benötigen. 
    /// Hinweis: Die Klasse <see cref="HeartRateReceiver"/> ist im Manifest.xml als BroadcastReceiver registiert.
    /// </summary>
    /// <seealso cref="Android.Content.BroadcastReceiver" />
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "HEARD_RATE_UPDATE" })]
    public class HeartRateReceiver : BroadcastReceiver
    {      
        private static MainActivity mainActivity;
        private static HRV hrv;

        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse <see cref="HeartRateReceiver"/>.
        /// Hinweis: Ein leerer Konstuktor wird vom Android System benötigt.
        /// </summary>
        public HeartRateReceiver(){ }

        /// <summary>
        /// Initialisiert eine neue Instanz der Klasse <see cref="HeartRateReceiver"/>.
        /// </summary>
        /// <param name="mainActivity">Die MainActivity.</param>
        /// <param name="hrv">Der HRV.</param>
        public HeartRateReceiver(MainActivity mainActivity, HRV hrv)
        {
            HeartRateReceiver.mainActivity = mainActivity;
            HeartRateReceiver.hrv = hrv;
        }

        /// <summary>
        /// Die Methode wird aufgerufen wenn der BroadcastReceiver einen entsprächenden Broadcast-Intent empfängt.
        /// </summary>
        /// <param name="context">Der Kontext in welchem der Receiver ausgeführt wird.</param>
        /// <param name="intent">Der empfangene Intent.</param>
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