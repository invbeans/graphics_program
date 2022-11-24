using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsk_course_work
{
    internal class Polygon : Figure
    {
        public const int Height = 90;
        float Xmin;
        float Xmax;
        float Ymin;
        float Ymax;

        public Polygon(Color color, Graphics g) : base(color, g) { }

        public void CalcTrianglePoints(Point p)
        {
            VertexList = new List<PointF>();
            VertexList.Add(new Point(p.X, p.Y - Height / 2));
            VertexList.Add(new Point(p.X + Height / 3, p.Y + Height / 2));
            VertexList.Add(new Point(p.X - Height / 3, p.Y + Height / 2));
        }

        public void CalcFlagPoints(Point p)
        {
            VertexList = new List<PointF>();
            VertexList.Add(new Point(p.X - Height, p.Y - Height / 2));
            VertexList.Add(new Point(p.X + Height, p.Y - Height / 2));
            VertexList.Add(new Point(p.X + Height / 3, p.Y));
            VertexList.Add(new Point(p.X + Height, p.Y + Height / 2));
            VertexList.Add(new Point(p.X - Height, p.Y + Height / 2));
        }

        public override void DrawFigure()
        {
            // Последовательно просматривая список вершин
            // находим границы Ymin и Ymax по оси OY
            int Ymin = (int)VertexList.Min(item => item.Y);
            int Ymax = (int)VertexList.Max(item => item.Y);
            // Создаем список для хранения точек пересечения 
            // сторон мнг-ка со строками Y
            List<int> Xb = new List<int>();
            // Вычисление точки пересечения 
            for (int j = Ymin; j <= Ymax; j++)
            {
                Xb.Clear();
                for (int i = 0; i < VertexList.Count; i++)
                {
                    int k;
                    if (i < VertexList.Count - 1) k = i + 1;
                    else k = 0;
                    if ((VertexList[i].Y < j && VertexList[k].Y >= j)
                        || (VertexList[i].Y >= j && VertexList[k].Y < j))
                    {
                        int x = (int)Math.Ceiling((double)(VertexList[k].X - VertexList[i].X) * (j - VertexList[i].Y) /
                            (double)(VertexList[k].Y - VertexList[i].Y) + VertexList[i].X);
                        // Добавляем точку пересечения в список 
                        Xb.Add(x);
                    }
                }
                // Сортировка списка 
                Xb.Sort();
                for (int i = 0; i < Xb.Count - 1; i += 2)
                {
                    // Закрашивание многоугольника
                    G.DrawLine(new Pen(Color), new Point(Xb[i], j), new Point(Xb[i + 1], j));
                }
            }
        }

        public override bool ThisFigure(Point p)
        {
            int k = 0, m = 0;
            PointF Pi, Pk;
            int n = VertexList.Count;
            for (int i = 0; i < n; i++)
            {
                if (i < n - 1) k = i + 1; else k = 0;
                Pi = VertexList[i]; Pk = VertexList[k];
                if ((Pi.Y < p.Y) & (Pk.Y >= p.Y) | (Pi.Y >= p.Y) & (Pk.Y < p.Y))
                    if ((p.Y - Pi.Y) * (Pk.X - Pi.X) / (Pk.Y - Pi.Y) + Pi.X < p.X) m++;
            }
            return (m % 2 == 1);
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

        public override void MoveFigure(int dx, int dy)
        {
            PointF temp = new Point();
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
            PointF center = new PointF
            {
                X = Xmin + ((Xmax - Xmin) / 2),
                Y = Ymin + ((Ymax - Ymin) / 2)
            };
            return center;
        }

        public override void RotateFigure(int diff, PointF center)
        {
            PointF temp = new PointF();
            double cos = Math.Cos(diff * Math.PI / 180);
            double sin = Math.Sin(diff * Math.PI / 180);
            for(int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)(cos * (VertexList[i].X - center.X) - sin * (VertexList[i].Y - center.Y) + center.X);
                temp.Y = (float)(sin * (VertexList[i].X - center.X) + cos * (VertexList[i].Y - center.Y) + center.Y);
                VertexList[i] = temp;
            }
        }

        public override void PointReflection(PointF center)
        {
            PointF temp = new PointF();
            for(int i = 0; i < VertexList.Count; i++)
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
