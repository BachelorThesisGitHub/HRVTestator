using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using HRVTestator.Views;

namespace HRVTestator
{
    [Activity(Label = "Herzratenvariabilität", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private enum Views { Circle, Line, Empty, Start }
        private PolarH7Service polarH7Service;
        private HeartRateReceiver broadcastReceiver;

        private PhonecallReceiver phonecallReceiver;

        private CircleHeartRateView circleHeartRateView;
        private LineHeartRateView lineHeartRateView;
        private EmptyHeartRateView emptyHeartRateView;
        private Views currentView = Views.Empty;
        private Context context;
        private AnalysisPlanner analysisPlanner;
        private List<Tuple<DateTime, string>> mesures = new List<Tuple<DateTime, string>>();

        private IMenu optionsMenu; //Da das Optionmenu nicht unmittelbar geladen wird, muss es als Klassenvariable zwischengespeichert werden.
        private int amountOfValuesToCalculateHRV; //der Parameter / UV
        private int firstValuestoIgnore;
        private List<int> heartRateValues = new List<int>();
        private int lastRRValue; //der zuletzt berechnete RR-Wert, wird für den Filter verwendet
        private bool showValues = false;
        int count = 0;

        protected override void OnCreate(Bundle bundle)
        {
            context = base.BaseContext;
            bool request = RequestWindowFeature(WindowFeatures.ActionBar);
            base.OnCreate(bundle);
            circleHeartRateView = new CircleHeartRateView(this);
            lineHeartRateView = new LineHeartRateView(this);
            emptyHeartRateView = new EmptyHeartRateView(this);

            SetView(currentView);

            broadcastReceiver = new HeartRateReceiver(this);
            IntentFilter filter = new IntentFilter();
            RegisterReceiver(broadcastReceiver, filter);

            phonecallReceiver = new PhonecallReceiver();

            analysisPlanner = new AnalysisPlanner(this);

            ActionBar actionBar = this.ActionBar;
            actionBar.Show();

            DefineAmountToCalculateHRV();       
        }

        private void DefineAmountToCalculateHRV()
        {
            var layout = new LinearLayout(this);
            layout.Orientation = Orientation.Horizontal;

            var label = new TextView(this);
            label.Text = "Parameter:";
            label.SetMinimumWidth(550); //550
            var textview = new EditText(this);
            textview.Text = "";

            textview.SetMinimumWidth(200);
            var button = new Button(this);
            button.Text = "OK";

            layout.AddView(label);
            layout.AddView(textview);
            layout.AddView(button);
            SetContentView(layout);

            button.Click += (sender, e) =>
            {
                if (Int32.TryParse(textview.Text, out amountOfValuesToCalculateHRV))
                {
                    layout.Visibility = Android.Views.ViewStates.Invisible;
                    firstValuestoIgnore = amountOfValuesToCalculateHRV;
                    OnCreateOptionsMenu(optionsMenu);
                }
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            optionsMenu = menu;
            if (amountOfValuesToCalculateHRV != 0) // Menu wird erst sichtbar, wenn der Parameter definiert ist
            {
                base.OnCreateOptionsMenu(menu);
                menu.Add(0, (int)Views.Start, 0, "Start");
                //menu.Add(0, (int)Views.Circle, 1, "Kreis");
                menu.Add(0, (int)Views.Line, 2, "Linie");
            }
            return true;
        }

        public void UpdateHeartRateView(int[] heartRate)
        {
            //Toast.MakeText(this, "data recieved", ToastLength.Short).Show(); 
            foreach (int rrValue in heartRate)
            {

                if (!IsValueValid(rrValue))
                {
                    continue; // RR-Differenz von mehr als 100ms rausfiltern
                }

                float hrv = CalulateHRV(rrValue);
                
                //circleHeartRateView.UpdateHeartRate(hrv);
                //circleHeartRateView.Invalidate();

                 // Die ersten Werte sollen nicht angezeigt werden.
                //if (firstValuestoIgnore > 0)
                //{
                //    firstValuestoIgnore -= 1;
                //    return;
                //}

                
                mesures.Add(new Tuple<DateTime, string>(DateTime.UtcNow, "   ; " + rrValue + "    ; " + hrv));

                if (showValues)
                {               
                    lineHeartRateView.showLine = true;
                    lineHeartRateView.ReceivedNewHRVValue();
                    lineHeartRateView.UpdateHeartRate(hrv);
                    lineHeartRateView.Invalidate();
                }
            }
        }

        private bool IsValueValid(int actualValue)
        {
            bool isValueValide = true;
            if (lastRRValue - actualValue > 100 || actualValue - lastRRValue > 100)
            {
                isValueValide = false;
                mesures.Add(new Tuple<DateTime, string>(DateTime.UtcNow, "Ungüliger Wert: Letzter Wert " + lastRRValue + " Aktueller Wert: " + actualValue));
            }
            lastRRValue = actualValue;
            return isValueValide;
        }

        private float CalulateHRV(int rrValue)
        {
            heartRateValues.Add(rrValue);
            float hrv = 0;
            List<float> squaredDiffOfNeigbours = new List<float>();

            if (heartRateValues.Count >= amountOfValuesToCalculateHRV + 1)
            {
                int negativCounter = -1 * ((int)amountOfValuesToCalculateHRV);
                for (int i = negativCounter; i < 0; i++)
                {
                    // Jedes Einzelne Differenzpaar berechnen           
                    float squaredDiffOfNeighbours = (heartRateValues[heartRateValues.Count + (i)]) - (heartRateValues[heartRateValues.Count + (i - 1)]);
                    squaredDiffOfNeigbours.Add(squaredDiffOfNeighbours);
                }

                float squaredSum = 0;
                // Die Summe der einzelnen quadrierten Differenzpaare berechnen
                foreach (float squaredDiffOfNeighbour in squaredDiffOfNeigbours)
                {
                    squaredSum += (float)Math.Pow(squaredDiffOfNeighbour, 2);
                }

                // Die Wurzel daraus ziehen
                hrv = (float)Math.Sqrt((1 / ((float)amountOfValuesToCalculateHRV - 1)) * squaredSum); ;
                Console.Out.WriteLine("HRV:....... " + String.Format("{0:0.00000}", hrv));
                heartRateValues.RemoveAt(0);
            }

            return hrv;
        }

        //public void DeleteMesures()
        //{
        //    mesures.RemoveRange(0, mesures.Count - AmountOfValuesToCalculateHRV);
        //}

        public void AddTextToMesurment(string text)
        {
            mesures.Add(new Tuple<DateTime, string>(DateTime.UtcNow, text));
        }

        public int GetAmountOfValuesToCalculateHRV()
        {
            return amountOfValuesToCalculateHRV;
        }

        public void ShowSymbols()
        {          
            Toast.MakeText(this, "show symbols", ToastLength.Short).Show();
            showValues = true;
            SetView(MainActivity.Views.Line);        
        }

        public void DeleteValuesToDraw()
        {
            lineHeartRateView.ClearHeartRateView();
        }

        public void StopAnalysis()
        {
            Toast.MakeText(this, "stop analysis", ToastLength.Short).Show();
            Email.Send(this, mesures);
            Toast.MakeText(this, "result saved", ToastLength.Short).Show();
            showValues = false;
        }

        public void HideSymbols()
        {
            Toast.MakeText(this, "hide symbols", ToastLength.Short).Show();
            SetView(MainActivity.Views.Empty);
            showValues = false;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            SetView((Views)item.ItemId);
            currentView = Views.Line;
            // lineHeartRateView.DeletePoints(); TODO 
            //mesures.Clear(); //Löschen der Daten, bei einer Eingabe im Menü --> Ab dann beginnt die Aufzeichnung
            return true;
        }


        private void SetView(Views currentView)
        {
            switch (currentView)
            {
                //case Views.Circle:
                //    SetContentView(circleHeartRateView);
                //    currentView = Views.Circle;
                //    return;
                case Views.Line:
                    lineHeartRateView.showLine = false;
                    SetContentView(lineHeartRateView);
                    currentView = Views.Line;
                    showValues = true;
                    return;
                case Views.Empty:
                    lineHeartRateView.showLine = false;
                    SetContentView(emptyHeartRateView);
                    currentView = Views.Empty;
                    showValues = false;
                    
                    return;
                case Views.Start:
                    lineHeartRateView.showLine = false;
                    lineHeartRateView.ClearPoints();
                    Toast.MakeText(this, "Analyse started", ToastLength.Short).Show();
                    showValues = false;
                    analysisPlanner.Start();
                    return;
            }

            throw new InvalidOperationException("Could not cast MenuItemId: " + currentView);
        }

        protected override void OnDestroy()
        {
            try
            {
                //UnregisterReceiver(broadcastReceiver);
                //StopService(new Intent(this, typeof(PolarH7Service)));
                if (polarH7Service != null)
                {
                    polarH7Service.Dispose();
                    //polerH7Service.StopService()
                }
                //broadcastReceiver.Dispose();
                base.OnDestroy();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("Absturz bei 269");
            }
        }


        protected override void OnStart()
        {
            base.OnStart();

            polarH7Service = new PolarH7Service(this);

            polarH7Service.Dispose();

            //StartService(new Intent(this, typeof(PolarH7Service)));
            polarH7Service.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            
            polarH7Service.Dispose();
        }




    }

}