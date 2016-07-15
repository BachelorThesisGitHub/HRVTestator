using Android.Content;
using Android.Graphics;

namespace HRVTestator.Gui
{
    public class EmptyHeartRateView : AbstractHeartRateView
    {
        public EmptyHeartRateView(Context context) : base(context) { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }
    }
}