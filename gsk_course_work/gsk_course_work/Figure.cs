using System;
using System.Collections.Generic;
using System.Drawing;

namespace gsk_course_work
{
    internal abstract class Figure
    {
        public Color Color { get; set; }
        public List<PointF> VertexList { get; set; }
        public Graphics G;
        public abstract void DrawFigure();
        public abstract bool ThisFigure(Point p);
        public abstract void DrawSelection();
        public abstract PointF GetCenter();
        public abstract void MoveFigure(int dx, int dy);
        public abstract void RotateFigure(int diff, PointF center);
        public abstract void PointReflection(PointF center);
        public abstract void VerLineReflection(PointF center);

        public Figure(Color color, Graphics g)
        {
            Color = color;
            G = g;
        }
    }
}
