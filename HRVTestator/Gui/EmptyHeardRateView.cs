using Android.Content;
using Android.Graphics;

namespace HRVTestator.Gui
{
    /// <summary>
    /// Die Klasse <see cref="EmptyHeartRateView"/> zeigt einen leere View mit dem Metronom an ohne Messresultate. 
    /// </summary>
    /// <seealso cref="HRVTestator.Gui.AbstractHeartRateView" />
    public class EmptyHeartRateView : AbstractHeartRateView
    {
        /// <summary>
        /// Instanziert eine neue Instanz der Klasse <see cref="EmptyHeartRateView"/>.
        /// </summary>
        public EmptyHeartRateView(Context context) : base(context) { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
        }
    }
}