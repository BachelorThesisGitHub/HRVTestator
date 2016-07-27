using Android.Content;
using Android.Views;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HRVTestator.Gui
{
    /// <summary>
    /// Die Abstrakte Klasse <see cref="AbstractHeartRateView"/> dient als Basisklasse für die verschiedenen Darstellungsformen der Messresultate.
    /// </summary>
    /// <seealso cref="Android.Views.View" />
    /// <seealso cref="HRVTestator.Gui.IInvalidatable" />
    public abstract class AbstractHeartRateView : View, IInvalidatable
    {
        private List<float> mesuredHeartRate = new List<float>();
        private List<int> stackOfNewValues = new List<int>();
        private Metronom metronom;

        /// <summary>
        /// Instanziert eine neue Instanz der Klasse <see cref="AbstractHeartRateView"/>.
        /// </summary>
        /// <param name="context">Der Kontext.</param>
        public AbstractHeartRateView(Context context) : base(context)
        {
            metronom = new Metronom(this);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawBackground(canvas);
            DrawBmsText(canvas);

            Rect rect = new Rect();
            base.GetDrawingRect(rect);
            metronom.OnDraw(canvas, rect);
            base.OnDraw(canvas); // ????
        }

        /// <summary>
        /// Invalidatet die gesamte View.
        /// Dies bewirkt, dass die View neu erstellt und angezeigt wird.
        /// </summary>
        public void Invalidate()
        {
            PostInvalidate();
        }

        private void DrawBackground(Canvas canvas)
        {
            Paint paint = new Paint();
            paint.SetStyle(Paint.Style.Fill);
            paint.Color = Color.CornflowerBlue;
            canvas.DrawPaint(paint);
        }

        private void DrawBmsText(Canvas canvas)
        {
            Paint paint = new Paint();
            if (!HasSomeHeartRateValue())
            {
                return;
            }

            paint.Color = Color.Black;
            paint.TextSize = 100f;
            //canvas.DrawText(string.Format("{0} HRV", GetLastMesuredHeartRate()), 10, 100, paint); Zum Testen, HRV anzeigen
        }

        protected bool HasNewHRVValues()
        {
            if(stackOfNewValues.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Messungen zurück.
        /// </summary>
        public int CountOfValuesInStock()
        {
            return mesuredHeartRate.Count;
        }

        /// <summary>
        /// Setzt ein neuen HRV-Wert.
        /// </summary>
        public void ReceivedNewHRVValue()
        {
            stackOfNewValues.Add(1);
        }

        protected void ConsumedNewHRVValue()
        {
            if (stackOfNewValues.Count > 0)
            {
                stackOfNewValues.RemoveAt(stackOfNewValues.Count - 1);
            }
        }

        /// <summary>
        /// Setzt ein neuen Heart-Rate-Wert.
        /// </summary>
        /// <param name="heartRate">The heart rate.</param>
        public void UpdateHeartRate(float heartRate)
        {
            mesuredHeartRate.Add(heartRate);
        }

        /// <summary>
        /// Bereinigt die HeartRateView.
        /// </summary>
        public void ClearHeartRateView()
        {
            mesuredHeartRate.Clear();
        }

        /// <summary>
        /// Prüft ob genau eine Messung existiert.
        /// </summary>
        /// <returns>
        ///   <c>true</c> falls genau eine Messung existiert; sonst, <c>false</c>.
        /// </returns>
        public bool IsFirstMesuredHeartRate()
        {
            return mesuredHeartRate.Count == 1;
        }

        protected float GetFirstMesuredHeartRate()
        {
            return mesuredHeartRate.First();
        }

        protected bool HasSomeHeartRateValue()
        {
            return mesuredHeartRate.Count > 2;
        }

        protected float GetSecondLastMesuredHeartRate()
        {
            return mesuredHeartRate.ElementAt(mesuredHeartRate.Count - 2);
        }

        protected float GetLastMesuredHeartRate()
        {
            return mesuredHeartRate.Last();
        }

        protected float GetAverageHeartRate()
        {
            return (float)mesuredHeartRate.Average();
        }
    }
}