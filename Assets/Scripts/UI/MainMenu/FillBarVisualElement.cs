using UnityEngine;
using UnityEngine.UIElements;

namespace MyVisualElements
{
    [UxmlElement]
    [ExecuteAlways]
    public partial class FillBarVisualElement : VisualElement
    {
        [UxmlAttribute] public Color fillColor = Color.red;
        [UxmlAttribute] public Color strokeColor = Color.black;
        [UxmlAttribute] public int middleLinesNumber = 5;
        [UxmlAttribute] public float fillPercentage = 0.5f;

        public FillBarVisualElement()
        {
            generateVisualContent += onVisualContentGenerated;
        }

        public void onVisualContentGenerated(MeshGenerationContext ctx)
        {
            Painter2D painter = ctx.painter2D;
            Rect rect = contentRect;

            painter.strokeColor = strokeColor;
            painter.fillColor = fillColor;

            // Filling
            float radius = contentRect.height / 2;
            float maxFillWidth = contentRect.width - radius * 2;
            float fillWidth = maxFillWidth * fillPercentage;

            painter.BeginPath();

            if (fillPercentage > 0)
            {
                painter.Arc(new Vector2(radius, radius), radius, 90, 270);
            }

            painter.LineTo(new Vector2(radius + fillWidth, 0));

            if (fillPercentage >= 1f)
            {
                painter.Arc(new Vector2(radius + fillWidth, radius), radius, 270, 90);
            }
            else
            {
                painter.LineTo(new Vector2(radius + fillWidth, contentRect.height));
            }

            painter.LineTo(new Vector2(radius, contentRect.height));

            painter.ClosePath();
            painter.Fill();

            // Stroke
            painter.lineWidth = 10;
            painter.BeginPath();
            painter.Arc(new Vector2(contentRect.height / 2, contentRect.height / 2), contentRect.height / 2, 90, 270);
            painter.LineTo(new Vector2(contentRect.width - contentRect.height / 2, 0));
            painter.Arc(new Vector2(contentRect.width - contentRect.height / 2, contentRect.height / 2), contentRect.height / 2, 270, 90);
            painter.LineTo(new Vector2(contentRect.height / 2, contentRect.height));
            painter.ClosePath();
            painter.Stroke();

            // Lines
            for (int i = 0; i < middleLinesNumber; i++)
            {
                float x = contentRect.width / (middleLinesNumber + 1) * (i + 1);
                painter.lineWidth = 8;
                painter.BeginPath();
                painter.MoveTo(new Vector2(x, 0));
                painter.LineTo(new Vector2(x, contentRect.height));
                painter.Stroke();
            }
        }
    }
}


