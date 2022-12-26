namespace gsk_course_work
{
    internal class TMOLine
    {
        //левая и правая граница отрезка (координаты X) и строка (координата Y)
        public int xLeft;
        public int xRight;
        public int y;

        public TMOLine(int xLeft, int xRight, int y)
        {
            this.xLeft = xLeft;
            this.xRight = xRight;
            this.y = y;
        }

        public TMOLine(TMOLine other)
        {
            this.xLeft = other.xLeft;
            this.xRight = other.xRight;
            this.y = other.y;
        }
    }
}
