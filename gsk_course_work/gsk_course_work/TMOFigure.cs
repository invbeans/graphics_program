using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gsk_course_work
{
    internal class TMOFigure
    {
        public int FigA { get; set; }
        public int FigB { get; set; }
        public int TMOCode { get; set; }

        public TMOFigure(int figA, int figB, int TMOCode)
        {
            FigA = figA;
            FigB = figB;
            //FigA.inTMO = true; FigB.inTMO = true;

            this.TMOCode = TMOCode;
        }

        /*static int[] SumSetQ = { 1, 3 };
        static int[] MinSetQ = { 2, 2 };*/
        /*public void DrawTMO()
        {
            int[] SetQ;
            SetQ = (TMOCode == 1) ? new int[] { 1, 3 } : new int[] { 2, 2 };
            Ymin = Math.Min(FigA.VertexList.Min(item => item.Y), FigB.VertexList.Min(item => item.Y));
            Ymax = Math.Max(FigA.VertexList.Max(item => item.Y), FigB.VertexList.Max(item => item.Y));
            FigA.Color = Color.CadetBlue;
            FigB.Color = Color.CadetBlue;
            FigA.DrawFigure();
            FigB.DrawFigure();
        }*/


    }
}
