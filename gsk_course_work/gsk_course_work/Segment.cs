using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsk_course_work
{
    internal class Segment : Figure
    {
        int Xmin;
        int Xmax;
        int Ymin;
        int Ymax;

        public Segment(Color color, Graphics g): base(color, g) { }

        public override void DrawFigure()
        {
            Pen pen = new Pen(Color);
            G.DrawLine(pen, VertexList[0], VertexList[1]);
        }

        public void GetSelection()
        {
            Xmin = VertexList.Min(p => p.X);
            Xmax = VertexList.Max(p => p.X);
            Ymin = VertexList.Min(p => p.Y);
            Ymax = VertexList.Max(p => p.Y);
        }

        public override void DrawSelection()
        {
            GetSelection();
            float[] dashPattern = { 5, 5 };
            Pen selectPen = new Pen(Color.Gray);
            selectPen.DashPattern = dashPattern;
            G.DrawLine(selectPen, new Point(Xmin, Ymax), new Point(Xmin, Ymin));
            G.DrawLine(selectPen, new Point(Xmin, Ymin), new Point(Xmax, Ymin));
            G.DrawLine(selectPen, new Point(Xmax, Ymin), new Point(Xmax, Ymax));
            G.DrawLine(selectPen, new Point(Xmax, Ymax), new Point(Xmin, Ymax));
        }

        public override bool ThisFigure(Point p)
        {
            Point Pi = VertexList[0], Pk = VertexList[1];
            if ((Pi.Y - 5 <= p.Y) & (Pk.Y + 5 >= p.Y) | (Pi.Y + 5 >= p.Y) & (Pk.Y - 5 <= p.Y))
            {
                int x;
                if (Pi.Y == Pk.Y) x = Pi.X;
                else x = (Pk.X - Pi.X) * (p.Y - Pi.Y) / (Pk.Y - Pi.Y) + Pi.X;
                if (x >= p.X - 5 & x <= p.X + 5) return true;
            }
            return false;
        }

        public override void MoveFigure(int dx, int dy)
        {
            Point temp = new Point();
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = VertexList[i].X + dx;
                temp.Y = VertexList[i].Y + dy;
                VertexList[i] = temp;
            }
        }
    }
}
