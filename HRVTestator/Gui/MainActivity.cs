using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using System;

namespace HRVTestator.Gui
{
    [Activity(Label = "Herzratenvariabilität", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ActivityWithMenu
    {
        private PolarH7Device polarH7Service;
        private HeartRateReceiver broadcastReceiver;
        private Context context;
        private AnalysisPlaner analysisPlanner;

        protected override void OnCreate(Bundle bundle)
        {
            // Need to be called before base.OnCreate()
            RequestWindowFeature(WindowFeatures.ActionBar);

            base.OnCreate(bundle);

            context = base.BaseContext;
            broadcastReceiver = new HeartRateReceiver(this, hrv);
            IntentFilter filter = new IntentFilter();
            RegisterReceiver(broadcastReceiver, filter);
            analysisPlanner = new AnalysisPlaner(this, hrv);

            ActionBar actionBar = this.ActionBar;
            actionBar.Show();      
        }

        public void UpdateHeartRateView(float hrvValue)
        {
            if (hrv.IsAmountOfValuesToCalculateHRVSet() && showValues)
            {
                lineHeartRateView.ReceivedNewHRVValue();
                lineHeartRateView.UpdateHeartRate(hrvValue);
                lineHeartRateView.Invalidate();
            }
        }

        protected override void StartAnalysis()
        {
            analysisPlanner.Start();
        }

        public void ShowSymbols()
        {
            lineHeartRateView.ClearHeartRateView();
            showValues = true;
            SetView(Views.Line);        
        }

        public void DeleteValuesToDraw()
        {
            lineHeartRateView.ClearHeartRateView();
        }

        public void StopAnalysis()
        {
            showValues = false;
        }

        public void HideSymbols()
        {
            SetView(Views.Empty);
            showValues = false;
        }

        protected override void OnDestroy()
        {
            try
            {
                if (polarH7Service != null)
                {
                    polarH7Service.Dispose();
                }

                base.OnDestroy();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Absturz bei 269: " + ex);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            polarH7Service = new PolarH7Device(this);
            polarH7Service.Dispose();
            polarH7Service.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            polarH7Service.Dispose();
        }
    }
}