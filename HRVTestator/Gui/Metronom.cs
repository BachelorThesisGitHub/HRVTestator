using Android.Graphics;
using System.Threading;

namespace HRVTestator.Gui
{
    public class Metronom
    {
        private enum trendOfMetronom { growing, shrinking }

        private const int PERIOD_IN_MILISEC = 35;
        private IInvalidatable invalidate;
        private Timer timer;
        private trendOfMetronom actualTrendOfMetronom;
        private int lastRadiusOfMetronomCircle;
        private bool hasMetronomStarted = false;

        public Metronom(IInvalidatable invalidate)
        {
            this.invalidate = invalidate;
        }

        public void OnDraw(Canvas canvas, Rect drawingRect)
        {
            if (!hasMetronomStarted)
            {
                StartMetronom();
                hasMetronomStarted = true;
            }

            Paint paint = new Paint();
            paint.Color = Color.Beige;
            canvas.DrawCircle(drawingRect.Width() / 2, 
                drawingRect.Height() / 2, 
                CalculateRadiusOFMetronom(lastRadiusOfMetronomCircle), 
                paint);
        }

        private int CalculateRadiusOFMetronom(int radius)
        {
            int newRadiusOFMetronomCircle;
            if (actualTrendOfMetronom == trendOfMetronom.growing)
            {
                newRadiusOFMetronomCircle = lastRadiusOfMetronomCircle + 5; // 5 (Camy's Smartphone)
            }
            else
            {
                newRadiusOFMetronomCircle = lastRadiusOfMetronomCircle - 5; //-5 (Camy's Smartphone)
            }

            if (lastRadiusOfMetronomCircle >= 715) //143 (Camy's Smartphone)
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

        private void StartMetronom()
        {
            TimerCallback timerDelegate = new TimerCallback((object state) => invalidate.Invalidate());
            timer = new Timer(timerDelegate, this, 0, PERIOD_IN_MILISEC);
        }
    }
}