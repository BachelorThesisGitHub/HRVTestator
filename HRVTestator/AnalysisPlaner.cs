using Android.OS;
using Android.Widget;
using HRVTestator.Gui;

namespace HRVTestator
{ 
    public class AnalysisPlaner
    {
        private MainActivity mainActivity;
        private Handler handler = new Handler();
        private int startPhase1 = 300000; //15000;
        private int startPhase2 = 600000; //30000;
        private int startPhase3 = 900000; //45000;
        private HRV hrv;

        public AnalysisPlaner(MainActivity mainActivty, HRV hrv)
        {
            this.mainActivity = mainActivty;
            this.hrv = hrv;
        }

        public void Start()
        {
            // Hide Symbols.
            this.hrv.SetPhase(Measurement.EnumPhase.Pre);
            Toast.MakeText(mainActivity, "hide symbols", ToastLength.Short).Show();
            mainActivity.HideSymbols();

            // Show Symbols after 5 Min.          
            handler.PostDelayed(() => 
            {
                this.hrv.SetPhase(Measurement.EnumPhase.Exp);
                mainActivity.DeleteValuesToDraw();
                Toast.MakeText(mainActivity, "show symbols", ToastLength.Short).Show();
                mainActivity.ShowSymbols();
            }, startPhase1);

            // Hide Symbols after 10 Min.
            handler.PostDelayed(() => 
            {
                this.hrv.SetPhase(Measurement.EnumPhase.Post);
                Toast.MakeText(mainActivity, "hide symbols", ToastLength.Short).Show();
                mainActivity.HideSymbols();
            }, startPhase2);

            // Stop Analysis after 15 minutes and send data mail.
            handler.PostDelayed(() =>
            {
                Toast.MakeText(mainActivity, "show symbols", ToastLength.Short).Show();
                mainActivity.ShowSymbols();
                Toast.MakeText(mainActivity, "stop analysis", ToastLength.Short).Show();
                Email.Send(mainActivity, hrv.GetFormatedMesurement());
                Toast.MakeText(mainActivity, "result saved", ToastLength.Short).Show();
                mainActivity.StopAnalysis();
            }, startPhase3);
        }
    }
}