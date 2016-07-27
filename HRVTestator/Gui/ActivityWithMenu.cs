using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace HRVTestator.Gui
{
    /// <summary>
    /// Die Abstrakte Klasse <see cref="ActivityWithMenu"/> dient als Basisklasse für die verschiedenen Activities der APP.
    /// </summary>
    /// <seealso cref="Android.App.Activity" />
    public abstract class ActivityWithMenu : Activity
    {
        public enum Views { Line, Empty, Start }
        private Views currentView = Views.Empty;
        private IMenu optionsMenu; // Da das Optionmenu nicht unmittelbar geladen wird, muss es als Klassenvariable zwischengespeichert werden.
        protected HRV hrv = new HRV();

        protected bool showValues = false;
        protected LineHeartRateView lineHeartRateView;
        protected EmptyHeartRateView emptyHeartRateView;

        protected abstract void StartAnalysis();

        /// <summary>
        /// Initilisiert die ActionBar der Activity
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            optionsMenu = menu;
            if (hrv.IsAmountOfValuesToCalculateHRVSet())
            {
                base.OnCreateOptionsMenu(menu);
                menu.Add(0, (int)Views.Start, 0, "Start");
                menu.Add(0, (int)Views.Line, 2, "Linie");
            }

            DefineAmountToCalculateHRV();

            return true;
        }

        private void DefineAmountToCalculateHRV()
        {
            var layout = new LinearLayout(this);
            layout.Orientation = Orientation.Horizontal;

            var label = new TextView(this);
            label.Text = "Parameter:";
            label.SetMinimumWidth(550);
            var textview = new EditText(this);
            textview.Text = "";

            textview.SetMinimumWidth(200);
            var button = new Button(this);
            button.Text = "OK";

            layout.AddView(label);
            layout.AddView(textview);
            layout.AddView(button);
            SetContentView(layout);
            int amountOfValuesToCalculateHRV;
            button.Click += (sender, e) =>
            {
                if (Int32.TryParse(textview.Text, out amountOfValuesToCalculateHRV))
                {
                    hrv.SetAmountOfValuesToCalutateHRV(amountOfValuesToCalculateHRV);
                    layout.Visibility = ViewStates.Invisible;
                    OnCreateOptionsMenu(optionsMenu);
                }
            };
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            lineHeartRateView = new LineHeartRateView(this);
            emptyHeartRateView = new EmptyHeartRateView(this);

            SetView(currentView);
        }

        /// <summary>
        /// Wird aufgerufen sobald in der ActionBar eine Auswahl getroffen wurde.
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            SetView((Views)item.ItemId);
            currentView = Views.Line;
            return true;
        }

        protected void SetView(Views currentView)
        {
            switch (currentView)
            {
                case Views.Line:
                    SetContentView(lineHeartRateView);
                    currentView = Views.Line;
                    showValues = true;
                    return;
                case Views.Empty:
                    SetContentView(emptyHeartRateView);
                    currentView = Views.Empty;
                    showValues = false;
                    return;
                case Views.Start:
                    Toast.MakeText(this, "Analyse started", ToastLength.Short).Show();
                    showValues = false;
                    StartAnalysis();
                    return;
            }

            throw new InvalidOperationException("Could not cast MenuItemId: " + currentView);
        }
    }
}