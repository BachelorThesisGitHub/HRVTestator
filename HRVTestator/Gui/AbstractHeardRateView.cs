using Android.Content;
using Android.Views;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HRVTestator.Gui
{
    public abstract class AbstractHeartRateView : View, IInvalidatable
    {
        private List<float> mesuredHeartRate = new List<float>();
        private List<int> stackOfNewValues = new List<int>();
        private Metronom metronom;

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

        public int CountOfValuesInStock()
        {
            return mesuredHeartRate.Count;
        }

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

        public void UpdateHeartRate(float heartRate)
        {
            mesuredHeartRate.Add(heartRate);
        }

        public void ClearHeartRateView()
        {
            mesuredHeartRate.Clear();
        }

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