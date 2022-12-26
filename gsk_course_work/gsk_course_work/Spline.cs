using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        //метод рисования сплайна
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
                    //сохранение полученных точек в списке
                    drawPoints.Add(Pt);
                    G.DrawLine(pen, Ppred, Pt);
                    Ppred = Pt;
                    t += dt;
                }
                newPoints = false;
            }
            else
            {
                //если это повторное рисование сплайна, используются ранее сохраненные точки
                Pen pen = new Pen(Color);
                for(int i = 0; i < drawPoints.Count - 1; i++)
                {
                    G.DrawLine(pen, drawPoints[i], drawPoints[i + 1]);
                }
            }
        }

        //метод получения точек для выделения
        public void GetSelection()
        {
            //находятся максимальные и минимальные X и Y для рисования выделения (описанный четырёхугольник)
            Xmin = drawPoints.Min(p => p.X);
            Xmax = drawPoints.Max(p => p.X);
            Ymin = drawPoints.Min(p => p.Y);
            Ymax = drawPoints.Max(p => p.Y);
        }

        //метод рисования выделения (описанного четырёхугольника)
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

        //метод проверки нажатия на сплайн
        public override bool ThisFigure(Point point)
        {
            int k;
            PointF Pi, Pk;
            int n = drawPoints.Count;
            for (int i = 0; i < n; i++)
            {
                if (i < n - 1) k = i + 1; else k = 0;
                Pi = drawPoints[i]; Pk = drawPoints[k];
                if((Pi.Y <= point.Y) & (Pk.Y >= point.Y) | (Pi.Y >= point.Y) & (Pk.Y <= point.Y))
                {
                    float x;
                    if (Pi.Y == Pk.Y) x = Pi.X;
                    else x = (Pk.X - Pi.X) * (point.Y - Pi.Y) / (Pk.Y - Pi.Y) + Pi.X;
                    //допускается отступ от координаты X на 5 пикселей в любую сторону
                    if (x >= point.X - 5 & x <= point.X + 5) return true;
                }
            }
            return false;
        }

        //метод перемещения сплайна
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

        //метод получения центра сплайна
        public override PointF GetCenter()
        {
            GetSelection();
            //используется центр описанного четырёхугольника
            PointF center = new PointF
            {
                X = Xmin + ((Xmax - Xmin) / 2),
                Y = Ymin + ((Ymax - Ymin) / 2)
            };
            return center;
        }

        //метод поворота сплайна
        public override void RotateFigure(int diff, PointF point)
        {
            PointF temp = new PointF();
            double cos = Math.Cos(diff * Math.PI / 180);
            double sin = Math.Sin(diff * Math.PI / 180);
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)(cos * (drawPoints[i].X - point.X) - sin * (drawPoints[i].Y - point.Y) + point.X);
                temp.Y = (float)(sin * (drawPoints[i].X - point.X) + cos * (drawPoints[i].Y - point.Y) + point.Y);
                drawPoints[i] = temp;
            }
        }

        //метод зеркального отражения относительно точки
        public override void PointReflection(PointF point)
        {
            PointF temp = new PointF();
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)((drawPoints[i].X - point.X) * (-1) + point.X);
                temp.Y = (float)((drawPoints[i].Y - point.Y) * (-1) + point.Y);
                drawPoints[i] = temp;
            }
        }

        //метод зеркального отражения относительно вертикальной прямой
        public override void VerLineReflection(PointF point)
        {
            PointF temp = new PointF();
            for (int i = 0; i < drawPoints.Count; i++)
            {
                temp.X = (float)((drawPoints[i].X - point.X) * (-1) + point.X);
                temp.Y = drawPoints[i].Y;
                drawPoints[i] = temp;
            }
        }
    }
}
