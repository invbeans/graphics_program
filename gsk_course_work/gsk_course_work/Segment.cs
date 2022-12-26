using System;
using System.Drawing;

namespace gsk_course_work
{
    internal class Segment : Figure
    {
        public Segment(Color color, Graphics g): base(color, g) { }

        //метод рисования отрезка
        public override void DrawFigure()
        {
            Pen pen = new Pen(Color);
            G.DrawLine(pen, VertexList[0], VertexList[1]);
        }

        //метод для проверки нажатия на отрезок
        public override bool ThisFigure(Point point)
        {
            PointF Pi = VertexList[0], Pk = VertexList[1];
            //допускается отступ от координат X и Y на 5 пикселей в любую сторону
            if ((Pi.Y - 5 <= point.Y) & (Pk.Y + 5 >= point.Y) | (Pi.Y + 5 >= point.Y) & (Pk.Y - 5 <= point.Y))
            {
                float x;
                if (Pi.Y == Pk.Y) x = Pi.X;
                else x = (Pk.X - Pi.X) * (point.Y - Pi.Y) / (Pk.Y - Pi.Y) + Pi.X;
                if (x >= point.X - 5 & x <= point.X + 5) return true;
            }
            return false;
        }

        //метод рисования выделения (описанного четырёхугольника)
        public override void DrawSelection()
        {
            float[] dashPattern = { 5, 5 };
            Pen selectPen = new Pen(Color.Gray);
            selectPen.DashPattern = dashPattern;
            G.DrawLine(selectPen, VertexList[0].X, VertexList[0].Y, VertexList[1].X, VertexList[0].Y);
            G.DrawLine(selectPen, VertexList[1].X, VertexList[0].Y, VertexList[1].X, VertexList[1].Y);
            G.DrawLine(selectPen, VertexList[1].X, VertexList[1].Y, VertexList[0].X, VertexList[1].Y);
            G.DrawLine(selectPen, VertexList[0].X, VertexList[1].Y, VertexList[0].X, VertexList[0].Y);
        }

        //метод перемещения отрезка
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

        //метод получения центра отрезка
        public override PointF GetCenter()
        {
            PointF center = new PointF
            {
                X = (VertexList[0].X - VertexList[1].X) / 2 + VertexList[1].X,
                Y = (VertexList[0].Y - VertexList[1].Y) / 2 + VertexList[1].Y
            };
            return center;
        }

        //метод поворота отрезка
        public override void RotateFigure(int diff, PointF point)
        {
            PointF temp = new PointF();
            double cos = Math.Cos(diff * Math.PI / 180);
            double sin = Math.Sin(diff * Math.PI / 180);
            for (int i = 0; i < VertexList.Count; i++)
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
            for (int i = 0; i < VertexList.Count; i++)
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
