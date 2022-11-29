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

        //получение точек для построения треугольника вокруг выбранного центра
        public void CalcTrianglePoints(Point p)
        {
            VertexList = new List<PointF>();
            VertexList.Add(new Point(p.X, p.Y - Height));
            VertexList.Add(new Point(p.X + Height / 2, p.Y + Height));
            VertexList.Add(new Point(p.X - Height / 2, p.Y + Height));
        }

        //получение точек для построения флага вокруг выбранного центра
        public void CalcFlagPoints(Point p)
        {
            VertexList = new List<PointF>();
            VertexList.Add(new Point(p.X - Height, p.Y - Height / 2));
            VertexList.Add(new Point(p.X + Height, p.Y - Height / 2));
            VertexList.Add(new Point(p.X + Height / 3, p.Y));
            VertexList.Add(new Point(p.X + Height, p.Y + Height / 2));
            VertexList.Add(new Point(p.X - Height, p.Y + Height / 2));
        }

        //метод рисования многоугольника
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

        //метод для проверки нажатия на многоугольник
        public override bool ThisFigure(Point point)
        {
            int k = 0, m = 0;
            PointF Pi, Pk;
            int n = VertexList.Count;
            for (int i = 0; i < n; i++)
            {
                if (i < n - 1) k = i + 1; else k = 0;
                Pi = VertexList[i]; Pk = VertexList[k];
                if ((Pi.Y < point.Y) & (Pk.Y >= point.Y) | (Pi.Y >= point.Y) & (Pk.Y < point.Y))
                    if ((point.Y - Pi.Y) * (Pk.X - Pi.X) / (Pk.Y - Pi.Y) + Pi.X < point.X) m++;
            }
            return (m % 2 == 1);
        }

        //метод получения точек для выделения
        public void GetSelection()
        {
            //находятся максимальные и минимальные X и Y для рисования выделения (описанный четырёхугольник)
            Xmin = VertexList.Min(p => p.X);
            Xmax = VertexList.Max(p => p.X);
            Ymin = VertexList.Min(p => p.Y);
            Ymax = VertexList.Max(p => p.Y);
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

        //метод перемещения многоугольника
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

        //метод получения центра многоугольника
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

        //метод поворота многоугольника
        public override void RotateFigure(int diff, PointF point)
        {
            PointF temp = new PointF();
            double cos = Math.Cos(diff * Math.PI / 180);
            double sin = Math.Sin(diff * Math.PI / 180);
            for(int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)(cos * (VertexList[i].X - point.X) - sin * (VertexList[i].Y - point.Y) + point.X);
                temp.Y = (float)(sin * (VertexList[i].X - point.X) + cos * (VertexList[i].Y - point.Y) + point.Y);
                VertexList[i] = temp;
            }
        }

        //метод зеркального отражения относительно точки
        public override void PointReflection(PointF point)
        {
            PointF temp = new PointF();
            for(int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)((VertexList[i].X - point.X) * (-1) + point.X);
                temp.Y = (float)((VertexList[i].Y - point.Y) * (-1) + point.Y);
                VertexList[i] = temp;
            }
        }

        //метод зеркального отражения относительно вертикальной прямой
        public override void VerLineReflection(PointF point)
        {
            PointF temp = new PointF();
            for (int i = 0; i < VertexList.Count; i++)
            {
                temp.X = (float)((VertexList[i].X - point.X) * (-1) + point.X);
                temp.Y = VertexList[i].Y;
                VertexList[i] = temp;
            }
        }
    }
}
