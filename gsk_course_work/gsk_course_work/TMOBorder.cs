namespace gsk_course_work
{
    internal class TMOBorder
    {
        //координата X границы и приращение пороговой функции для этой границы
        public int x;
        public int dQ;

        public TMOBorder(int x, int dQ) { 
            this.x = x; 
            this.dQ = dQ; 
        }
    }
}
