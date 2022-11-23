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
        float Xmin;
        float Xmax;
        float Ymin;
        float Ymax;

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
            G.DrawLine(selectPen, Xmin, Ymax, Xmin, Ymin);
            G.DrawLine(selectPen, Xmin, Ymin, Xmax, Ymin);
            G.DrawLine(selectPen, Xmax, Ymin, Xmax, Ymax);
            G.DrawLine(selectPen, Xmax, Ymax, Xmin, Ymax);
        }

        public override bool ThisFigure(Point p)
        {
            PointF Pi = VertexList[0], Pk = VertexList[1];
            if ((Pi.Y - 5 <= p.Y) & (Pk.Y + 5 >= p.Y) | (Pi.Y + 5 >= p.Y) & (Pk.Y - 5 <= p.Y))
            {
                float x;
                if (Pi.Y == Pk.Y) x = Pi.X;
                else x = (Pk.X - Pi.X) * (p.Y - Pi.Y) / (Pk.Y - Pi.Y) + Pi.X;
                if (x >= p.X - 5 & x <= p.X + 5) return true;
            }
            return false;
        }

        public override void MoveFigure(int dx, int dy)
        {
            PointF temp = new PointF();
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = VertexList[i].X + dx;
                temp.Y = VertexList[i].Y + dy;
                VertexList[i] = temp;
            }
        }

        public override PointF GetCenter()
        {
            GetSelection();
            PointF center = new PointF();
            center.X = Xmin + ((Xmax - Xmin) / 2);
            center.Y = Ymin + ((Ymax - Ymin) / 2);
            return center;
        }

        public override void RotateFigure(int diff, PointF center)
        {
            PointF temp = new PointF();
            double cos = Math.Cos(diff * Math.PI / 180);
            double sin = Math.Sin(diff * Math.PI / 180);
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)(cos * (VertexList[i].X - center.X) - sin * (VertexList[i].Y - center.Y) + center.X);
                temp.Y = (float)(sin * (VertexList[i].X - center.X) + cos * (VertexList[i].Y - center.Y) + center.Y);
                VertexList[i] = temp;
            }
        }

        public override void PointReflection(PointF center)
        {
            PointF temp = new PointF();
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)((VertexList[i].X - center.X) * (-1) + center.X);
                temp.Y = (float)((VertexList[i].Y - center.Y) * (-1) + center.Y);
                VertexList[i] = temp;
            }
        }

        public override void VerLineReflection(PointF center)
        {
            PointF temp = new PointF();
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)((VertexList[i].X - center.X) * (-1) + center.X);
                temp.Y = VertexList[i].Y;
                VertexList[i] = temp;
            }
        }
    }
}
