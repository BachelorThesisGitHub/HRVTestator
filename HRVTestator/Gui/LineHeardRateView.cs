using Android.Content;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace HRVTestator.Gui
{
    /// <summary>
    /// Die Klasse <see cref="LineHeartRateView"/> ist verantwortliche für die Anzeige der empfangenen Messresultate. 
    /// </summary>
    /// <seealso cref="HRVTestator.Gui.AbstractHeartRateView" />
    public class LineHeartRateView : AbstractHeartRateView
    {
        private const float lineLenght = 10;
        private const float offsetX = 20;
        private const float pointSize = 20;
        private List<Point> points = new List<Point>();
        private const float scaleFactor = 10;
        public bool showLine = false;

        /// <summary>
        /// Instanziert eine neue Instanz der Klasse <see cref="LineHeartRateView"/>.
        /// </summary>
        public LineHeartRateView(Context context) : base(context) { }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            DrawLine(canvas);
        }
                
        private void DrawLine(Canvas canvas)
        {
            Rect rect = new Rect();
            base.GetDrawingRect(rect);
            Paint paint = new Paint();
            paint.Color = Color.Coral;

            if (HasSomeHeartRateValue()) //Wurden überhaupt schon Werte übermittelt?
            {

                if (points.Count == 0)
                {
                    //ClearHeartRateView(); // Bereits erfasste Punkte sollen nicht dargestellt werden- 
                    points.Add(GetStartPoint());
                }

                if (points.Last().X + lineLenght > rect.Width())
                {
                    points.Clear();
                    return;
                }

                if (HasNewHRVValues()) // Sind neue Werte hinzugekommen und noch nicht dargestellt?
                {
                    var nextPoint = GetNextPoint(points.Last(), GetDifferenceBetweenLastTwoValues());
                    points.Add(nextPoint);
                    canvas.DrawCircle(nextPoint.X, nextPoint.Y, pointSize + 10, paint);
                    ConsumedNewHRVValue();
                }

                for (int i = 0; i < points.Count - 1; i++)
                {
                    canvas.DrawLine(points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y, paint);
                    canvas.DrawCircle(points[i].X, points[i].Y, pointSize, paint);
                }
            }
        }

        private float GetDifferenceBetweenLastTwoValues()
        {
            return GetSecondLastMesuredHeartRate() - GetLastMesuredHeartRate();
        }

        private Point GetNextPoint(Point lastPoint, float difference)
        {
            return new Point((int)(lastPoint.X + lineLenght), (int)(lastPoint.Y + (difference * scaleFactor))); //?Convert
        }

        private Point GetStartPoint()
        {
            Rect rect = new Rect();
            base.GetDrawingRect(rect);
            return new Point((int)offsetX, rect.Height() / 2); //?Convert
        }
    }
}