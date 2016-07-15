using Android.Content;
using Android.Views;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace HRVTestator.Views
{
    public abstract class AbstractHeartRateView : View
    {
        protected Paint paint;
        private List<float> mesuredHeartRate = new List<float>();
        private List<int> stackOfNewValues = new List<int>();
        private enum trendOfMetronom { growing, shrinking }
        private trendOfMetronom actualTrendOfMetronom;
        private int lastRadiusOfMetronomCircle;
        private bool hasMetronomStarted = false;
        //int radius;
        //Canvas canvas;

        public AbstractHeartRateView(Context context) : base(context)
        {
            paint = new Paint();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawBackground(canvas);
            DrawBmsText(canvas);

            Rect rect = new Rect();
            base.GetDrawingRect(rect);

            if (!hasMetronomStarted)
            {
                StartMetronom();
                hasMetronomStarted = true;
            }

            paint.Color = Color.Beige;
            canvas.DrawCircle(rect.Width() / 2, rect.Height() / 2, CalculateRadiusOFMetronom(lastRadiusOfMetronomCircle), paint);
            base.OnDraw(canvas);
        }

        private void StartMetronom()
        {
            Metronom metronom = new Metronom();
            //Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate = new TimerCallback(Refresh);
            //Create a timer that waits one second, then invokes every second.
            Timer timer = new Timer(timerDelegate, metronom, 0, 35);//15 // x/1000*5*60000 = 4.28*2*700 --> x = 20

            //Keep a handle to the timer, so it can be disposed.
            metronom.tmr = timer;
        }

        private void Refresh(Object state)
        {
            PostInvalidate();
        }

        private int CalculateRadiusOFMetronom(int radius)
        {
            int newRadiusOFMetronomCircle;
            if (actualTrendOfMetronom == trendOfMetronom.growing)
            {
                newRadiusOFMetronomCircle = lastRadiusOfMetronomCircle + 5; // 5
            }
            else
            {
                newRadiusOFMetronomCircle = lastRadiusOfMetronomCircle - 5; //-5
            }

            if (lastRadiusOfMetronomCircle >= 715) //143)//715
            {
                actualTrendOfMetronom = trendOfMetronom.shrinking;
            }

            if (lastRadiusOfMetronomCircle <= 0)
            {
                actualTrendOfMetronom = trendOfMetronom.growing;
            }

            lastRadiusOfMetronomCircle = newRadiusOFMetronomCircle;
            return newRadiusOFMetronomCircle;
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
            if(mesuredHeartRate.Count > 15)
            {
                mesuredHeartRate.RemoveRange(0, mesuredHeartRate.Count - 15);
            }
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