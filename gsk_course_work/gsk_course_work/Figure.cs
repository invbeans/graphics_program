using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsk_course_work
{
    internal abstract class Figure
    {
        public Color Color { get; set; }
        //public bool IsPoligon { get; set; }
        public List<Point> VertexList { get; set; }
        public Graphics G;
        public abstract void DrawFigure();
        public abstract bool ThisFigure(Point p);
        public abstract void DrawSelection();
        public abstract void MoveFigure(int dx, int dy);

        public Figure(Color color, Graphics g)
        {
            Color = color;
            G = g;
        }
    }
}
