using Android.App;
using Android.Content;
using System;

namespace HRVTestator
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "HEARD_RATE_UPDATE" })]
    public class HeartRateReceiver : BroadcastReceiver
    {
        
        private static MainActivity mainActivity;

        public HeartRateReceiver() { }

        public HeartRateReceiver(MainActivity mainActivity)
        {
            HeartRateReceiver.mainActivity = mainActivity;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                int[] rrValues = intent.GetIntArrayExtra("HEARD_RATE");
                if (mainActivity != null)
                {
                    mainActivity.UpdateHeartRateView(rrValues);
                }
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("Fehler beim empfangen der Daten:.....e.InnerException");
            }

        }
    }
}