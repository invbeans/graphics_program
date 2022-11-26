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
        public Figure FigA { get; set; }
        public Figure FigB { get; set; }
        public int TMOCode { get; set; }
        public int canvasWidth;
        Graphics G;
        int[] SetQ;
        float Ymin;
        float Ymax;
        List<TMOLine> lines = new List<TMOLine>();
        float selXmin;
        float selYmin;
        float selXmax;
        float selYmax;

        public TMOHandler(Figure figA, Figure figB, int TMOCode, int canvasHeight, Graphics g)
        {
            this.FigA = figA;
            this.FigB = figB;
            this.TMOCode = TMOCode;
            this.canvasWidth = canvasHeight;
            G = g;
        }

        public void DrawTMO()
        {
            SetQ = (TMOCode == 1) ? new int[] { 1, 3 } : new int[] { 2, 2 };
            Ymin = Math.Min(FigA.VertexList.Min(item => item.Y), FigB.VertexList.Min(item => item.Y));
            Ymax = Math.Max(FigA.VertexList.Max(item => item.Y), FigB.VertexList.Max(item => item.Y));
            //Вызывать в рисовании построчно с мина до макса поиск сечений
            //если есть, выполнить на сечении ТМО, заполнить список для новой фигуры
            FillLines();
            Pen drawPen = new Pen(FigA.Color);
            for(int i = 0; i < lines.Count; i++)
            {
                G.DrawLine(drawPen, lines[i].xLeft, lines[i].y, lines[i].xRight, lines[i].y);
            }
        }

        private void FillLines()
        {
            List<TMOLine> figALines;
            List<TMOLine> figBLines;
            List<TMOBorder> M;
            List<int> TMOLeftX;
            List<int> TMORightX;
            for (int i = (int)Ymin; i < Ymax; i++)
            {
                M = new List<TMOBorder>();
                figALines = new List<TMOLine>();
                figBLines = new List<TMOLine>();
                //вызываем искалку здесь
                FindFigureSegments(FigA, figALines, i);
                FindFigureSegments(FigB, figBLines, i);
                //пытаемся заполнить, если не пусто проводим тмо
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
                if (M.Count > 0)
                {
                    int Q = 0; int Qnew = 0;
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
                        if ((Q < SetQ[0] || Q > SetQ[1]) && (Qnew >= SetQ[0] && Qnew <= SetQ[1]))
                        {
                            TMOLeftX.Add(x);
                        }
                        if((Q >= SetQ[0] && Q <= SetQ[1]) && (Qnew < SetQ[0] || Qnew > SetQ[1]))
                        {
                            TMORightX.Add(x);
                        }
                        Q = Qnew;
                    }
                    if(Q >= SetQ[0] && Q <= SetQ[1])
                    {
                        TMORightX.Add(canvasWidth);
                    }
                    for(int k = 0; k < TMORightX.Count; k++)
                    {
                        lines.Add(new TMOLine(TMOLeftX[k], TMORightX[k], i));
                    }
                }
            }
        }

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

        public void GetSelection()
        {
            selXmin = Math.Min(FigA.VertexList.Min(p => p.X), FigB.VertexList.Min(p => p.X));
            selXmax = Math.Max(FigA.VertexList.Max(p => p.X), FigB.VertexList.Max(p => p.X));
            selYmin = Math.Min(FigA.VertexList.Min(p => p.Y), FigB.VertexList.Min(p => p.Y));
            selYmax = Math.Max(FigA.VertexList.Max(p => p.Y), FigB.VertexList.Max(p => p.Y));
        }

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
    }
}
