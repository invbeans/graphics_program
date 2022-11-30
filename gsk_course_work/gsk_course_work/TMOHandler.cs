using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace gsk_course_work
{
    internal class TMOHandler
    {
        public int canvasWidth;
        public TMOFigure TMOFigure;
        Graphics G;
        int[] SetQ;
        float Ymin;
        float Ymax;
        List<TMOLine> lines = new List<TMOLine>();
        float selXmin;
        float selYmin;
        float selXmax;
        float selYmax;

        public TMOHandler(TMOFigure TMOFigure, int canvasWidth, Graphics g)
        {
            this.TMOFigure = TMOFigure;
            this.canvasWidth = canvasWidth;
            G = g;
        }

        //метод рисования ТМО
        public void DrawTMO()
        {
            SetQ = (TMOFigure.TMOCode == 1) ? new int[] { 1, 3 } : new int[] { 2, 2 };
            //получение строк, в пределах которых может быть результат ТМО
            Ymin = Math.Min(TMOFigure.FigA.VertexList.Min(item => item.Y), TMOFigure.FigB.VertexList.Min(item => item.Y));
            Ymax = Math.Max(TMOFigure.FigA.VertexList.Max(item => item.Y), TMOFigure.FigB.VertexList.Max(item => item.Y));
            lines.Clear();
            FillLines();
            Pen drawPen = new Pen(TMOFigure.FigA.Color);
            TMOFigure.Lines = lines;
            for(int i = 0; i < lines.Count; i++)
            {
                G.DrawLine(drawPen, lines[i].xLeft, lines[i].y, lines[i].xRight, lines[i].y);
            }
        }

        //метод для заполнения списка отрезков, из которых состоит ТМО
        private void FillLines()
        {
            List<TMOLine> figALines;
            List<TMOLine> figBLines;
            List<TMOBorder> M;
            List<int> TMOLeftX;
            List<int> TMORightX;
            //цикл по строкам
            for (int i = (int)Ymin; i < Ymax; i++)
            {
                M = new List<TMOBorder>();
                figALines = new List<TMOLine>();
                figBLines = new List<TMOLine>();
                //заполнение списка сегментов фигур, лежащих на текущей строке
                FindFigureSegments(TMOFigure.FigA, figALines, i);
                FindFigureSegments(TMOFigure.FigB, figBLines, i);
                //заполнение списка M - границы сегментов двух фигур
                for(int j = 0; j < figALines.Count; j++)
                {
                    M.Add(new TMOBorder(figALines[j].xLeft, 2));
                }
                for (int j = 0; j < figALines.Count; j++)
                {
                    M.Add(new TMOBorder(figALines[j].xRight, -2));
                }
                for (int j = 0; j < figBLines.Count; j++)
                {
                    M.Add(new TMOBorder(figBLines[j].xLeft, 1));
                }
                for (int j = 0; j < figBLines.Count; j++)
                {
                    M.Add(new TMOBorder(figBLines[j].xRight, -1));
                }
                M.Sort((e1, e2) => e1.x.CompareTo(e2.x));
                TMOLeftX = new List<int>(); TMORightX = new List<int>();
                //если на данной строке имеются сечения фигур, то над ними выполняется ТМО
                if (M.Count > 0)
                {
                    int Q = 0; int Qnew = 0;
                    //исключительный случай - выход за левую границу формы
                    if (M[0].x >= 0 && M[0].dQ < 0)
                    {
                        TMOLeftX.Add(0);
                        Q = -M[0].dQ;
                    }
                    int x;
                    for (int k = 0; k < M.Count; k++)
                    {
                        x = M[k].x;
                        Qnew = Q + M[k].dQ;
                        //начало сегмента ТМО
                        if ((Q < SetQ[0] || Q > SetQ[1]) && (Qnew >= SetQ[0] && Qnew <= SetQ[1]))
                        {
                            TMOLeftX.Add(x);
                        }
                        //конец сегмента ТМО
                        if((Q >= SetQ[0] && Q <= SetQ[1]) && (Qnew < SetQ[0] || Qnew > SetQ[1]))
                        {
                            TMORightX.Add(x);
                        }
                        Q = Qnew;
                    }
                    //исключительный случай - выход за правую границу формы
                    if(Q >= SetQ[0] && Q <= SetQ[1])
                    {
                        TMORightX.Add(canvasWidth);
                    }
                    //сохранение сегментов ТМО на текущей строке
                    for(int k = 0; k < TMORightX.Count; k++)
                    {
                        lines.Add(new TMOLine(TMOLeftX[k], TMORightX[k], i));
                    }
                }
            }
        }

        //метод нахождения сегментов многоугольника на заданной строке Y
        private void FindFigureSegments(Figure figure, List<TMOLine> list, int Y)
        {
            List<int> xList = new List<int>();
            for (int i = 0; i < figure.VertexList.Count; i++)
            {
                int k;
                if (i < figure.VertexList.Count - 1) k = i + 1;
                else k = 0;
                if ((figure.VertexList[i].Y < Y && figure.VertexList[k].Y >= Y)
                    || (figure.VertexList[i].Y >= Y && figure.VertexList[k].Y < Y))
                {
                    int x = (int)Math.Ceiling((double)(figure.VertexList[k].X - figure.VertexList[i].X) * (Y - figure.VertexList[i].Y) /
                        (double)(figure.VertexList[k].Y - figure.VertexList[i].Y) + figure.VertexList[i].X);
                    xList.Add(x);
                }
            }
            xList.Sort();
            for(int i = 0; i < xList.Count - 1; i += 2)
            {
                list.Add(new TMOLine(xList[i], xList[i + 1], Y));
            }
        }

        //метод получения точек для выделения - используются исходные фигуры
        public void GetSelection()
        {
            int length = TMOFigure.Lines.Count;
            selXmin = Math.Min(TMOFigure.Lines.Min(p => p.xLeft), TMOFigure.Lines.Min(p => p.xRight));
            selXmax = Math.Max(TMOFigure.Lines.Max(p => p.xLeft), TMOFigure.Lines.Max(p => p.xRight));
            selYmin = Math.Min(TMOFigure.Lines.Min(p => p.y), TMOFigure.Lines.Min(p => p.y));
            selYmax = Math.Max(TMOFigure.Lines.Max(p => p.y), TMOFigure.Lines.Max(p => p.y));
        }

        //метод получения центра ТМО (центр описанного четырёхугольника)
        public PointF GetTMOCenter()
        {
            GetSelection();
            PointF center = new PointF
            {
                X = selXmin + ((selXmax - selXmin) / 2),
                Y = selYmin + ((selYmax - selYmin) / 2)
            };
            return center;
        }

        //метод рисования выделения (описанного четырёхугольника)
        public void DrawSelection()
        {
            GetSelection();
            float[] dashPattern = { 5, 5 };
            Pen selectPen = new Pen(Color.Gray);
            selectPen.DashPattern = dashPattern;
            G.DrawLine(selectPen, selXmin, selYmax, selXmin, selYmin);
            G.DrawLine(selectPen, selXmin, selYmin, selXmax, selYmin);
            G.DrawLine(selectPen, selXmax, selYmin, selXmax, selYmax);
            G.DrawLine(selectPen, selXmax, selYmax, selXmin, selYmax);
        }

        //метод проверки нажатия на фигуру
        public bool ThisFigure(Point point)
        {
            for(int i = 0; i < TMOFigure.Lines.Count; i++)
            {
                if(point.Y == TMOFigure.Lines[i].y)
                {
                    if(point.X >= TMOFigure.Lines[i].xLeft && point.X <= TMOFigure.Lines[i].xRight) return true;
                }
            }
            return false;
        }
    }
}
