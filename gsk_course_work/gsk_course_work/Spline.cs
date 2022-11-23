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
        private List<PointF> drawPoints = new List<PointF>();
        float Xmin;
        float Xmax;
        float Ymin;
        float Ymax;
        public Spline(Color color, Graphics g) : base(color, g) { }

        public override void DrawFigure()
        {
            if (newPoints)
            {
                // Матрица вещественных коэффициентов L
                PointF[] L = new PointF[4];

                // Касательные векторы
                PointF vector1 = VertexList[0];
                PointF vector2 = VertexList[0];

                const double dt = 0.04;
                double t = 0;
                double xt, yt;

                PointF Ppred = VertexList[0], Pt = VertexList[0];

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
            G.DrawLine(selectPen, Xmin, Ymax, Xmin, Ymin);
            G.DrawLine(selectPen, Xmin, Ymin, Xmax, Ymin);
            G.DrawLine(selectPen, Xmax, Ymin, Xmax, Ymax);
            G.DrawLine(selectPen, Xmax, Ymax, Xmin, Ymax);
        }

        public override bool ThisFigure(Point p)
        {
            int k = 0, m = 0;
            PointF Pi, Pk;
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
                    float x;
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
            PointF temp = new PointF();
            for(int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = drawPoints[i].X + dx;
                temp.Y = drawPoints[i].Y + dy;
                drawPoints[i] = temp;
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
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)(cos * (drawPoints[i].X - center.X) - sin * (drawPoints[i].Y - center.Y) + center.X);
                temp.Y = (float)(sin * (drawPoints[i].X - center.X) + cos * (drawPoints[i].Y - center.Y) + center.Y);
                drawPoints[i] = temp;
            }
        }

        public override void PointReflection(PointF center)
        {
            PointF temp = new PointF();
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)((drawPoints[i].X - center.X) * (-1) + center.X);
                temp.Y = (float)((drawPoints[i].Y - center.Y) * (-1) + center.Y);
                drawPoints[i] = temp;
            }
        }

        public override void VerLineReflection(PointF center)
        {
            PointF temp = new PointF();
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)((drawPoints[i].X - center.X) * (-1) + center.X);
                temp.Y = drawPoints[i].Y;
                drawPoints[i] = temp;
            }
        }
    }
}
