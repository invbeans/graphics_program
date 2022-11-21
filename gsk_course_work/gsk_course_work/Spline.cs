using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsk_course_work
{
    internal class Spline : Figure
    {
        private bool newPoints = true;
        private List<Point> drawPoints = new List<Point>();
        int Xmin;
        int Xmax;
        int Ymin;
        int Ymax;
        public Spline(Color color, Graphics g) : base(color, g) { }

        public override void DrawFigure()
        {
            if (newPoints)
            {
                // Матрица вещественных коэффициентов L
                PointF[] L = new PointF[4];

                // Касательные векторы
                Point vector1 = VertexList[0];
                Point vector2 = VertexList[0];

                const double dt = 0.04;
                double t = 0;
                double xt, yt;

                Point Ppred = VertexList[0], Pt = VertexList[0];

                vector1.X = 4 * (VertexList[1].X - VertexList[0].X);
                vector1.Y = 4 * (VertexList[1].Y - VertexList[0].Y);
                vector2.X = 4 * (VertexList[3].X - VertexList[2].X);
                vector2.Y = 4 * (VertexList[3].Y - VertexList[2].Y);

                // Расчет коэффициентов полинома
                L[0].X = 2 * VertexList[0].X - 2 * VertexList[2].X + vector1.X + vector2.X; // Ax
                L[0].Y = 2 * VertexList[0].Y - 2 * VertexList[2].Y + vector1.Y + vector2.Y; // Ay
                L[1].X = -3 * VertexList[0].X + 3 * VertexList[2].X - 2 * vector1.X - vector2.X; // Bx
                L[1].Y = -3 * VertexList[0].Y + 3 * VertexList[2].Y - 2 * vector1.Y - vector2.Y; // By
                L[2].X = vector1.X; // Cx
                L[2].Y = vector1.Y; // Cy
                L[3].X = VertexList[0].X; // Dx
                L[3].Y = VertexList[0].Y; // Dy
                Pen pen = new Pen(Color);
                drawPoints.Add(Ppred);
                
                while (t < 1 + dt / 2)
                {
                    xt = ((L[0].X * t + L[1].X) * t + L[2].X) * t + L[3].X;
                    yt = ((L[0].Y * t + L[1].Y) * t + L[2].Y) * t + L[3].Y;

                    Pt.X = (int)Math.Round(xt);
                    Pt.Y = (int)Math.Round(yt);

                    drawPoints.Add(Pt);
                    G.DrawLine(pen, Ppred, Pt);
                    Ppred = Pt;
                    t += dt;
                }
                newPoints = false;
            }
            else
            {
                Pen pen = new Pen(Color);
                for(int i = 0; i < drawPoints.Count - 1; i++)
                {
                    G.DrawLine(pen, drawPoints[i], drawPoints[i + 1]);
                }
            }
        }

        public void GetSelection()
        {
            Xmin = drawPoints.Min(p => p.X);
            Xmax = drawPoints.Max(p => p.X);
            Ymin = drawPoints.Min(p => p.Y);
            Ymax = drawPoints.Max(p => p.Y);
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
            int k = 0, m = 0;
            Point Pi, Pk;
            m = 0;
            int n = drawPoints.Count;
            for (int i = 0; i < n; i++)
            {
                if (i < n - 1) k = i + 1; else k = 0;
                Pi = drawPoints[i]; Pk = drawPoints[k];
                //if ((Pi.Y < p.Y) & (Pk.Y >= p.Y) | (Pi.Y >= p.Y) & (Pk.Y < p.Y))
                //if ((Pi.Y <= p.Y) & (Pk.Y >= p.Y) | (Pi.Y >= p.Y) & (Pk.Y <= p.Y))
                if((Pi.Y <= p.Y) & (Pk.Y >= p.Y) | (Pi.Y >= p.Y) & (Pk.Y <= p.Y))
                {
                    int x;
                    if (Pi.Y == Pk.Y) x = Pi.X;
                    else x = (Pk.X - Pi.X) * (p.Y - Pi.Y) / (Pk.Y - Pi.Y) + Pi.X;
                    if (x >= p.X - 5 & x <= p.X + 5) return true;
                }
                //if ((Pi.Y < p.Y) & (Pk.Y >= p.Y) | (Pi.Y >= p.Y) & (Pk.Y < p.Y))
                //  if ((p.Y - Pi.Y) * (Pk.X - Pi.X) / (Pk.Y - Pi.Y) + Pi.X < p.X) m++;
            }
            return false;
        }

        public override void MoveFigure(int dx, int dy)
        {
            Point temp = new Point();
            for(int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = drawPoints[i].X + dx;
                temp.Y = drawPoints[i].Y + dy;
                drawPoints[i] = temp;
            }
        }
    }
}
