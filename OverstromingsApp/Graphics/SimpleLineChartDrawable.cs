using Microsoft.Maui.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace OverstromingsApp.Graphics
{
    /// <summary>
    /// Eenvoudige line-chart-tekenaar met optionele jaar-scheidingslijnen.
    /// </summary>
    public class SimpleLineChartDrawable : IDrawable
    {
        public List<float> Values { get; set; } = new();
        public List<string> Labels { get; set; } = new();
        public List<string>? SecondLabels { get; set; } = null;     // optionele tweede regel

        public float ZoomFactor { get; set; } = 1f;
        public float PanOffset { get; set; } = 0f;

        /// <summary>
        /// Indexen (0-based) waar januari begint.  Voor elke index
        /// wordt een verticale hulplijn tussen december → januari getekend.
        /// </summary>
        public List<int> YearSeparators { get; set; } = new();

        public void Draw(ICanvas c, RectF rect)
        {
            if (Values.Count < 2) return;

            const float margin = 40;
            float chartW = rect.Width - margin * 2;
            float chartH = rect.Height - margin * 2;
            float step = (chartW / (Values.Count - 1)) * ZoomFactor;
            float maxVal = Values.Max() * 1.1f;
            float xStart = margin - PanOffset;

            /* ── achtergrond & assen ────────────────────────── */
            c.FillColor = Colors.White;
            c.FillRectangle(rect);
            c.StrokeColor = Colors.Black;
            c.StrokeSize = 1;
            c.DrawLine(margin, margin, margin, margin + chartH);                    // y-as
            c.DrawLine(margin, margin + chartH, rect.Width - margin, margin + chartH); // x-as

            /* ── jaar-scheidingslijnen ─────────────────────── */
            if (YearSeparators?.Count > 0)
            {
                c.StrokeColor = Colors.LightGray;
                c.StrokeSize = 1;
                foreach (int i in YearSeparators.Where(idx => idx > 0))
                {
                    float x = xStart + (i - 0.5f) * step;      // midden tussen dec en jan
                    if (x < margin || x > rect.Width - margin) continue;
                    c.DrawLine(x, margin, x, margin + chartH);
                }
            }

            /* ── hoofd-lijn ────────────────────────────────── */
            c.StrokeColor = Colors.Blue;
            c.StrokeSize = 2;
            for (int i = 0; i < Values.Count - 1; i++)
            {
                float x1 = xStart + i * step;
                float y1 = margin + chartH - (Values[i] / maxVal * chartH);
                float x2 = xStart + (i + 1) * step;
                float y2 = margin + chartH - (Values[i + 1] / maxVal * chartH);
                c.DrawLine(x1, y1, x2, y2);
            }

            /* ── punten + labels ───────────────────────────── */
            c.FillColor = Colors.Red;
            c.FontSize = 10;
            for (int i = 0; i < Values.Count; i++)
            {
                float x = xStart + i * step;
                if (x < margin || x > rect.Width - margin) continue;

                float y = margin + chartH - (Values[i] / maxVal * chartH);
                c.FillCircle(x, y, 3);
                c.DrawString(((int)Values[i]).ToString(), x - 10, y - 15, 40, 20,
                             HorizontalAlignment.Center, VerticalAlignment.Top);
                c.DrawString(Labels[i], x - 20, margin + chartH + 5, 60, 20,
                             HorizontalAlignment.Center, VerticalAlignment.Top);

                if (SecondLabels != null &&
                    i < SecondLabels.Count &&
                    !string.IsNullOrEmpty(SecondLabels[i]))
                {
                    c.DrawString(SecondLabels[i], x - 20, margin + chartH + 22, 60, 20,
                                 HorizontalAlignment.Center, VerticalAlignment.Top);
                }
            }

            /* ── horizontale hulplijnen ────────────────────── */
            c.StrokeColor = Colors.LightGray;
            for (int i = 0; i <= 4; i++)
            {
                float v = i * (maxVal / 4);
                float y = margin + chartH - (v / maxVal * chartH);
                c.DrawLine(margin, y, rect.Width - margin, y);
                c.DrawString(((int)v).ToString(), 0, y - 10, margin - 5, 20,
                             HorizontalAlignment.Right, VerticalAlignment.Center);
            }
        }
    }
}
