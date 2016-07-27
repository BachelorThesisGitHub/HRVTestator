using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using System;

namespace HRVTestator.Gui
{
    /// <summary>
    /// Die Klasse <see cref="MainActivity"/> ist der Gui-Kontainer der gesammten App. 
    /// Die App besteht aus lediglich einer Activity.
    /// </summary>
    /// <seealso cref="HRVTestator.Gui.ActivityWithMenu" />
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

        /// <summary>
        /// Updatet die HeartRateView.
        /// </summary>
        /// <param name="hrvValue">Der HRV-Wert</param>
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

        /// <summary>
        /// Zeigt die Symbole an.
        /// </summary>
        public void ShowSymbols()
        {
            lineHeartRateView.ClearHeartRateView();
            showValues = true;
            SetView(Views.Line);        
        }

        /// <summary>
        /// Löscht die zu zeichnenden Werte.
        /// </summary>
        public void DeleteValuesToDraw()
        {
            lineHeartRateView.ClearHeartRateView();
        }

        /// <summary>
        /// Stoppt die Analyse.
        /// </summary>
        public void StopAnalysis()
        {
            showValues = false;
        }

        /// <summary>
        /// Versteckt die zu Symbole.
        /// </summary>
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