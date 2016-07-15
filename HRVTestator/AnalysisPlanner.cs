using Android.OS;

namespace HRVTestator
{ 
    public class AnalysisPlanner
    {
        private MainActivity mainActivity;
        private Handler handler = new Handler();
        private int startPhase1 = 300000; //15000;
        private int startPhase2 = 600000; //30000;
        private int startPhase3 = 900000; //45000;

        public AnalysisPlanner(MainActivity mainActivty)
        {
            this.mainActivity = mainActivty;
        }

        public void Start()
        {
            // Hide Symbols.            
            mainActivity.AddTextToMesurment("UV: " + mainActivity.GetAmountOfValuesToCalculateHRV());
            mainActivity.AddTextToMesurment(" Vorhermessung");
            mainActivity.HideSymbols();

            // Show Symbols after 5 Min.          
            handler.PostDelayed(() => 
            {
                mainActivity.AddTextToMesurment(" Exp-Phase");
                mainActivity.DeleteValuesToDraw();
                mainActivity.ShowSymbols();
            }, startPhase1);

            // Hide Symbols after 10 Min.
            handler.PostDelayed(() => 
            {
                mainActivity.AddTextToMesurment(" Nachhermessung");
                mainActivity.HideSymbols();
            }, startPhase2);

            // Stop Analysis after 15 minutes and send data mail.
            handler.PostDelayed(() =>
            {
                mainActivity.ShowSymbols();
                mainActivity.StopAnalysis();
            }, startPhase3);
        }
    }
}