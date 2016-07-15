using Android.Content;
using Android.Graphics;

namespace HRVTestator.Views
{
    public class CircleHeartRateView : AbstractHeartRateView
    {
        public CircleHeartRateView(Context context) : base(context) { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (!HasSomeHeartRateValue())
            {
                return;
            }

            Rect rect = new Rect();
            base.GetDrawingRect(rect);

            paint.Color = Color.Coral;
            canvas.DrawCircle(
                rect.Width() / 2,
                rect.Height() / 2,
                CalculateRadiosFromHeardRate(GetLastMesuredHeartRate()),
                paint);
        }

        private float CalculateRadiosFromHeardRate(float heardRate)
        {
            const float offset = 10;
            const float scalFactor = 20;
            return (heardRate - GetAverageHeartRate() + offset) * scalFactor;
        }
    }
}