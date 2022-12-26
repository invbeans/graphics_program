using System.Collections.Generic;

namespace gsk_course_work
{
    internal class TMOFigure
    {
        public Figure FigA { get; set; }
        public Figure FigB { get; set; }
        public int TMOCode { get; set; }
        public List<TMOLine> Lines { get; set; }

        public TMOFigure(Figure figA, Figure figB, int TMOCode)
        {
            FigA = figA;
            FigB = figB;
            this.TMOCode = TMOCode;
        }
    }
}
